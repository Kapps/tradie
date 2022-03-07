// See https://aka.ms/new-console-template for more information

using Amazon.CloudWatch;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Indexer;
using Tradie.ItemLog;

var ssmClient = new AmazonSimpleSystemsManagementClient();
var s3Client = new AmazonS3Client();

await TradieConfig.InitializeFromEnvironment(ssmClient);

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
			.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
			.AddSingleton<IAmazonS3>(s3Client)
			.AddSingleton<IAmazonCloudWatch>(new AmazonCloudWatchClient())
			.AddSingleton<IMetricPublisher, CloudWatchMetricPublisher>()
			.AddSingleton<IItemLog, PostgresItemLog>();

		services.AddDbContext<AnalysisContext>(ServiceLifetime.Singleton);

	})
	.Build();

var itemLog = host.Services.GetRequiredService<IItemLog>();

Console.WriteLine($"Starting Indexer with build hash {Environment.GetEnvironmentVariable("BUILD_HASH")}");

Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

long startBytes = currentProcess.WorkingSet64;


var test = new IndexTest();
//await test.SearchByBruteForce(itemLog);
await test.SearchByBlocks(itemLog);
//test.SearchByBlocks();

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

currentProcess = System.Diagnostics.Process.GetCurrentProcess();
long endBytes = currentProcess.WorkingSet64;

test.PrintResults();

Console.WriteLine($"Started off with using {startBytes / (1024 * 1024)}MB, ended with {endBytes / (1024*1024)}MB.");