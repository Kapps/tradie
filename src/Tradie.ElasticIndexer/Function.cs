using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.IO.Compression;
using System.Net;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

// ReSharper disable AccessToDisposedClosure

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Tradie.ElasticIndexer;

public class Function {
	public async Task FunctionHandler(S3Event input, ILambdaContext context) {
		try {
			using var ssmClient = new AmazonSimpleSystemsManagementClient();
			using var s3Client = new AmazonS3Client();

			Console.WriteLine($"Starting Analyzer with build hash {Environment.GetEnvironmentVariable("BUILD_HASH")}");
			
			async Task Initializer() {
				await TradieConfig.InitializeFromEnvironment(ssmClient);
			}

			await Initializers.InitializeOnce(Initializer);

			using IHost host = Host.CreateDefaultBuilder()
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
						.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
						.AddSingleton<IAmazonS3>(s3Client);

					services.AddStackExchangeRedisCache(opts => {
						opts.ConfigurationOptions = new ConfigurationOptions() {
							User = TradieConfig.RedisUser,
							Password = TradieConfig.RedisPass,
							EndPoints = {
								new DnsEndPoint(TradieConfig.RedisHost, 6379)
							},
							ClientName = "Tradie ElasticIndexer"
						};
					});
					
					services.AddDbContext<AnalysisContext>(ServiceLifetime.Singleton);
				})
				.Build();

			Console.WriteLine($"Got input file {input.Records[0].S3.Object.Key}.");

			foreach(var record in input.Records) {
				Console.WriteLine($"Getting record for {record.S3.Bucket.Name}:{record.S3.Object.Key}");
				var stashes = await GetStashTabsFromRecord(record, s3Client);
				foreach(var stash in stashes) {
					Console.WriteLine($"Got stash {stash.Id} with {stash.Items.Length} items.");
				}
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