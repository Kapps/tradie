using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Tradie.Common;
using Tradie.Scanner;
using System.Threading.Tasks;

var environment = Environment.GetEnvironmentVariable("TRADIE_ENV") ??
          throw new ArgumentException("TRADIE_ENV environment variable not set.");
Console.WriteLine($"Env: {environment}");
var ssmClient = new AmazonSimpleSystemsManagementClient();
var s3Client = new AmazonS3Client();

await TradieConfig.InitializeFromSsm(environment, ssmClient);

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services => {
		services.AddLogging(builder => {
			builder.AddSimpleConsole(format => {
				format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
				format.UseUtcTimestamp = true;
				format.IncludeScopes = true;
				format.SingleLine = false;
			});
		});
		services.AddHostedService<ScannerWorker>();
		services.AddSingleton<IParameterStore, SsmParameterStore>()
			.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
			.AddSingleton<IAmazonS3>(s3Client)
			.AddSingleton<IApiClient, ApiClient>()
			.AddSingleton<IChangeSetParser, ChangeSetParser>()
			.AddSingleton<IChangeSetStore, S3ChangeSetStore>()
			.AddSingleton<ICompressor, BrotliCompressor>();
	})
	.Build();


await host.RunAsync();

