using Amazon.Kinesis;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Medallion.Threading.Postgres;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
			
			var distributedLock = new PostgresDistributedLock(
				new PostgresAdvisoryLockKey("ItemLogBuilder", true),
				AnalysisContext.CreateConnectionString()
			);
			
			await using var lockHandle = await distributedLock.TryAcquireAsync();
			if(lockHandle == null) {
				Console.WriteLine("Exiting early because a lock is already in place.");
				return;
			}

			//var mongoClient = new MongoClient(TradieConfig.MongoItemLogConnectionString);
			
			IHost host = Host.CreateDefaultBuilder()
				.ConfigureServices(services => {
					services.AddLogging(builder => {
						builder.AddSimpleConsole(format => {
							format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.mmm] ";
							format.UseUtcTimestamp = true;
							format.IncludeScopes = false;
							format.SingleLine = true;
						});
					});
					services.AddSingleton<IParameterStore, SsmParameterStore>()
						.AddSingleton<IAmazonKinesis, AmazonKinesisClient>()
						.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
						.AddSingleton<IAmazonS3>(s3Client)
						.AddSingleton<IItemLogBuilder, PostgresLogBuilder>()
						//.AddSingleton<IMongoClient>(mongoClient)
						//.AddSingleton<IMongoDatabase>(mongoClient.GetDatabase("tradie"))
						//.AddSingleton<IItemLogBuilder, MongoLogBuilder>()
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
			
			
			using var cancellationTokenSource = new CancellationTokenSource(context.RemainingTime - TimeSpan.FromSeconds(30));
				
			Console.WriteLine("Starting copy.");
			await streamer.CopyItemsFromLog(sourceLog, logBuilder, cancellationTokenSource.Token);

			//Console.WriteLine("Committing");
			//await tx.CommitAsync(cancellationTokenSource.Token);

			Console.WriteLine("Finished copy.");

		} catch(Exception ex) {
			Console.WriteLine("---Exception Details---");
			Console.WriteLine(ex.ToString());
			throw;
		}
	}
}