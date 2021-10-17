using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Tradie.Common;
using Tradie.Scanner;
using System.Threading.Tasks;

string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY") ?? throw new ArgumentNullException("Environment.AWS_ACCESS_KEY");
string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY") ?? throw new ArgumentNullException("Environment.AWS_SECRET_KEY");
string regionName = Environment.GetEnvironmentVariable("AWS_REGION") ?? throw new ArgumentNullException("Environment.AWS_REGION");
var creds = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
var region = RegionEndpoint.GetBySystemName(regionName);

var ssmClient = new AmazonSimpleSystemsManagementClient(creds, region);
var s3Client = new AmazonS3Client(creds, region);
var config = await TradieConfig.LoadFromSSM(ssmClient);

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services => {
		services.AddHostedService<ScannerWorker>();
		services.AddSingleton<IParameterStore, SsmParameterStore>()
			.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
			.AddSingleton<IAmazonS3>(s3Client)
			.AddSingleton<IApiClient, ApiClient>()
			.AddSingleton<IChangeSetParser, ChangeSetParser>()
			.AddSingleton<IChangeSetStore, S3ChangeSetStore>()
			.AddSingleton<TradieConfig>(config)
			.AddSingleton<ICompressor, BrotliCompressor>();
	})
	.Build();

await host.RunAsync();

