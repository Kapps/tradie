using Amazon.CloudWatch;
using Amazon.Kinesis;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Amazon.SQS;
using Medallion.Threading.Postgres;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.Metrics;
using Tradie.ItemLog;
using Tradie.ItemLogBuilder;
using Tradie.ItemLogBuilder.Elastic;
using Tradie.ItemLogBuilder.Postgres;


var ssmClient = new AmazonSimpleSystemsManagementClient();
var s3Client = new AmazonS3Client();

await TradieConfig.InitializeFromEnvironment(ssmClient);

Console.WriteLine($"Starting ItemLogBuilder with build hash {Environment.GetEnvironmentVariable("BUILD_HASH")}");

var distributedLock = new PostgresDistributedLock(
	new PostgresAdvisoryLockKey("ItemLogBuilder", true),
	AnalysisContext.CreateConnectionStringBuilder().ToString()
);

await using var lockHandle = await distributedLock.TryAcquireAsync();
if(lockHandle == null) {
	Console.WriteLine("Exiting early because a lock is already in place.");
	return;
}

IHost host = Host.CreateDefaultBuilder()
	.ConfigureServices(services => {
		services.AddLogging(builder => {
			builder.AddSimpleConsole(format => {
				format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.mmm] ";
				format.UseUtcTimestamp = true;
				format.IncludeScopes = false;
				format.SingleLine = true;
			});
			builder.SetMinimumLevel(TradieConfig.LogLevel);
		});
		services.AddSingleton<IParameterStore, SsmParameterStore>()
			.AddSingleton<IAmazonKinesis, AmazonKinesisClient>()
			.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
			.AddSingleton<IAmazonS3>(s3Client)
			.AddSingleton<IAmazonCloudWatch, AmazonCloudWatchClient>()
			.AddSingleton<IAmazonSQS, AmazonSQSClient>()
			.AddSingleton<IItemLogBuilder, PostgresLogBuilder>()
			//.AddSingleton<IItemLogBuilder, ElasticLogBuilder>()
			.AddSingleton<IElasticClient, TradieElasticClient>()
			.AddSingleton<IMetricPublisher, CloudWatchMetricPublisher>()
			.AddSingleton<ILoggedItemRepository, PostgresLoggedItemRepository>()
			.AddSingleton<ILoggedTabRepository, PostgresLoggedTabRepository>()
			.AddSingleton<ILogStreamer, LogStreamer>()
			.AddSingleton<IStashTabSerializer, MessagePackedStashTabSerializer>()
			.AddSingleton<IKinesisRecordReader, KinesisRecordReader>()
			.AddSingleton<IPoisonPillReporter, SqsPoisonPillReporter>(provider => new SqsPoisonPillReporter(
				provider.GetRequiredService<IAmazonSQS>(),
				TradieConfig.ItemStreamPoisonPillQueueUrl,
				provider.GetRequiredService<ILogger<SqsPoisonPillReporter>>()))
			.AddSingleton<IItemLog, KinesisItemLog>(provider => new KinesisItemLog(
				new KinesisStreamReference(TradieConfig.LogBuilderShardId,
					TradieConfig.AnalyzedItemStreamName),
				provider.GetRequiredService<IKinesisRecordReader>(),
				provider.GetRequiredService<IStashTabSerializer>(),
				provider.GetRequiredService<IPoisonPillReporter>()));

		services.AddDbContext<AnalysisContext>(ServiceLifetime.Singleton);

	})
	.Build();

var logBuilder = host.Services.GetRequiredService<IItemLogBuilder>();
var sourceLog = host.Services.GetRequiredService<IItemLog>();
var streamer = host.Services.GetRequiredService<ILogStreamer>();


using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(400.5));

Console.WriteLine("Starting copy.");
try {
	await streamer.CopyItemsFromLog(sourceLog, logBuilder, cancellationTokenSource.Token);
} catch(OperationCanceledException) {
	Console.WriteLine("Exiting cleanly due to a cancelled context.");
	return;
}

Console.WriteLine("Finished copy.");