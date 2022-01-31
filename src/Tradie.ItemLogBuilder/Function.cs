using Amazon.Kinesis;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Transactions;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.ItemLog;
using Tradie.ItemLogBuilder.Postgres;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Tradie.ItemLogBuilder; 


public class Function {
	public async Task FunctionHandler(ScheduledEvent input, ILambdaContext context) {
		try {
			var ssmClient = new AmazonSimpleSystemsManagementClient();
			var s3Client = new AmazonS3Client();

			await TradieConfig.InitializeFromEnvironment(ssmClient);
			
			Console.WriteLine($"Starting ItemLogBuilder with build hash {Environment.GetEnvironmentVariable("BUILD_HASH")}");
			
			IHost host = Host.CreateDefaultBuilder()
				.ConfigureServices(services => {
					services.AddLogging(builder => {
						builder.AddSimpleConsole(format => {
							format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
							format.UseUtcTimestamp = true;
							format.IncludeScopes = true;
							format.SingleLine = false;
						});
					});
					services.AddSingleton<IParameterStore, SsmParameterStore>()
						.AddSingleton<IAmazonKinesis, AmazonKinesisClient>()
						.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
						.AddSingleton<IAmazonS3>(s3Client)
						.AddSingleton<IItemLogBuilder, PostgresLogBuilder>()
						.AddSingleton<ILoggedItemRepository, PostgresLoggedItemRepository>()
						.AddSingleton<ILoggedTabRepository, PostgresLoggedTabRepository>()
						.AddSingleton<ILogStreamer, LogStreamer>()
						.AddSingleton<IStashTabSerializer, MessagePackedStashTabSerializer>()
						.AddSingleton<IKinesisRecordReader, KinesisRecordReader>()
						.AddSingleton<IItemLog, KinesisItemLog>(provider => new KinesisItemLog(
							new KinesisStreamReference(TradieConfig.LogBuilderShardId,
								TradieConfig.AnalyzedItemStreamName),
							provider.GetRequiredService<IKinesisRecordReader>(),
							provider.GetRequiredService<IStashTabSerializer>()
						));

					services.AddDbContext<AnalysisContext>(ServiceLifetime.Singleton);

				})
				.Build();

			var logBuilder = host.Services.GetRequiredService<IItemLogBuilder>();
			var sourceLog = host.Services.GetRequiredService<IItemLog>();
			var streamer = host.Services.GetRequiredService<ILogStreamer>();
			var dbContext = host.Services.GetRequiredService<AnalysisContext>();
			var conn = await dbContext.GetOpenedConnection<NpgsqlConnection>();
			await using var tx = await conn.BeginTransactionAsync();
			
			using var cancellationTokenSource = new CancellationTokenSource(context.RemainingTime - TimeSpan.FromSeconds(30));
			
			//while(!cancellationTokenSource.IsCancellationRequested) {
				//using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
				
				Console.WriteLine("Starting copy.");
				try {
					await streamer.CopyItemsFromLog(sourceLog, logBuilder, new ItemLogOffset(null),
						cancellationTokenSource.Token);
				} catch(OperationCanceledException) {
					
				}
				//scope.Complete();
			//}

			Console.WriteLine("Committing");
			await tx.CommitAsync(cancellationTokenSource.Token);

			Console.WriteLine("Finished copy.");

		} catch(Exception ex) {
			Console.WriteLine("---Exception Details---");
			Console.WriteLine(ex.ToString());
			throw;
		}
	}
}