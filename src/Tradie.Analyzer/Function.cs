using Amazon.Kinesis;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SimpleSystemsManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.IO.Compression;
using System.Net;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Tradie.Analyzer; 


public class Function {
	public async Task FunctionHandler(S3Event input, ILambdaContext context) {
		try {
			var ssmClient = new AmazonSimpleSystemsManagementClient();
			var s3Client = new AmazonS3Client();

			Console.WriteLine($"Starting Analyzer with build hash {Environment.GetEnvironmentVariable("BUILD_HASH")}");
			
			async Task Initializer() {
				await TradieConfig.InitializeFromEnvironment(ssmClient);
				await using var dbContext = new AnalysisContext();
				
				var remainingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
				Console.WriteLine($"Found {remainingMigrations.Count()} migrations to apply.");
				await dbContext.Database.MigrateAsync();
				Console.WriteLine("Finished applying migrations.");
			}

			await Initializers.InitializeOnce(Initializer);

			IHost host = Host.CreateDefaultBuilder()
				.ConfigureServices(services => {
					services.AddLogging(builder => {
						builder.AddSimpleConsole(format => {
							format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
							format.UseUtcTimestamp = true;
							format.IncludeScopes = true;
							format.SingleLine = false;
						});
						builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
					});
					services.AddSingleton<IParameterStore, SsmParameterStore>()
						.AddSingleton<IAmazonKinesis, AmazonKinesisClient>()
						.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
						.AddSingleton<IAmazonS3>(s3Client)
						.AddSingleton<IItemAnalyzer, ItemTypeAnalyzer>()
						.AddSingleton<IItemAnalyzer, ModifierAnalyzer>()
						.AddSingleton<IItemAnalyzer, AffixRangeAnalyzer>()
						.AddSingleton<IItemAnalyzer, ItemDetailsAnalyzer>()
						.AddSingleton<IItemAnalyzer, TradePropertiesAnalyzer>()
						.AddSingleton<IModConverter, AnalyzingModConverter>()
						.AddSingleton<IModifierRepository, ModifierDbRepository>()
						.AddSingleton<IAffixRangeRepository, AffixRangeRepository>()
						.AddSingleton<IPersistentEntityConverter<ItemType>, ItemTypeConverter>()
						.AddSingleton<IItemTypeRepository, ItemTypeDbRepository>()
						.AddSingleton<IAnalyzedStashTabDispatcher, AnalyzedStashTabKinesisDispatcher>()
						.AddSingleton<IStashTabSerializer, MessagePackedStashTabSerializer>();

					services.AddStackExchangeRedisCache(opts => {
						opts.ConfigurationOptions = new ConfigurationOptions() {
							User = TradieConfig.RedisUser,
							Password = TradieConfig.RedisPass,
							EndPoints = {
								new DnsEndPoint(TradieConfig.RedisHost, 6379)
							},
							ClientName = "Tradie Analyzer"
						};
					});
					
					services.AddDbContext<AnalysisContext>(ServiceLifetime.Transient);
				})
				.Build();

			Console.WriteLine($"Got input file {input.Records[0].S3.Object.Key}.");

			var itemAnalyzers = host.Services.GetServices<IItemAnalyzer>().ToArray();

			var stashAnalyzer = new StashTabAnalyzer(itemAnalyzers); // TODO: Can this become part of services?
			var dispatcher = host.Services.GetRequiredService<IAnalyzedStashTabDispatcher>();

			foreach(var record in input.Records) {
				Console.WriteLine($"Getting record for {record.S3.Bucket.Name}:{record.S3.Object.Key}");
				var stashes = await GetStashTabsFromRecord(record, s3Client);
				foreach(var stash in stashes) {
					Console.WriteLine($"Found stash with id {stash.Id} containing {stash.Items.Length} items.");
					var analyzedTab = await stashAnalyzer.AnalyzeTab(stash);
					await dispatcher.DispatchTab(analyzedTab);
				}

				await dispatcher.Flush();
			}
		} catch(Exception ex) {
			Console.WriteLine("---Exception Details---");
			Console.WriteLine(ex.ToString());
			throw;
		}
	}

	private static async Task<RawStashTab[]> GetStashTabsFromRecord(S3EventNotification.S3EventNotificationRecord record, IAmazonS3 s3Client) {
		var obj = await s3Client.GetObjectAsync(record.S3.Bucket.Name, record.S3.Object.Key);
		await using var jsonStream = obj.ResponseStream;
		await using var decompressedStream = new BrotliStream(jsonStream, CompressionMode.Decompress);

		var stashes = await SpanJson.JsonSerializer.Generic.Utf8.DeserializeAsync<RawStashTab[]>(decompressedStream);
		return stashes;
	}
}