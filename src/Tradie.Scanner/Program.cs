using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using RateLimiter;
using Tradie.Common;
using Tradie.Scanner;

var environment = Environment.GetEnvironmentVariable("TRADIE_ENV") ??
          throw new ArgumentException("TRADIE_ENV environment variable not set.");
Console.WriteLine($"Env: {environment}");
var ssmClient = new AmazonSimpleSystemsManagementClient();
var s3Client = new AmazonS3Client();

await TradieConfig.InitializeFromSsm(environment, ssmClient);

IHost host = Host.CreateDefaultBuilder(args)
	.UseConsoleLifetime()
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
			.AddSingleton<IApiClient, PoEApiClient>()
			.AddSingleton<IChangeSetParser, ChangeSetParser>()
			.AddSingleton<IChangeSetStore, S3ChangeSetStore>()
			.AddSingleton<ICompressor, BrotliCompressor>();
	})
	.Build();


Console.WriteLine("About to run at {0}", DateTime.Now);

// Intentionally wait a second before running, to prevent spamming the API on a crash.
await Task.Delay(1000);

try {
	await host.RunAsync();
} catch(Exception ex) {
	Console.WriteLine("Running the host encountered an error and is going to exit.");
	Console.WriteLine(ex.ToString());
}

Console.WriteLine("Exiting at {0}", DateTime.Now);

