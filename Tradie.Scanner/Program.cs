using Amazon;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Tradie.Common;
using Tradie.Scanner;


string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY") ?? throw new ArgumentNullException("Environment.AWS_ACCESS_KEY");
string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY") ?? throw new ArgumentNullException("Environment.AWS_SECRET_KEY");
string regionName = Environment.GetEnvironmentVariable("AWS_REGION") ?? throw new ArgumentNullException("Environment.AWS_REGION");
var creds = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
var region = RegionEndpoint.GetBySystemName(regionName);

var ssmClient = new AmazonSimpleSystemsManagementClient(creds, region);

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services => {
		services.AddHostedService<ScannerWorker>();
		services.AddSingleton<IParameterStore, SsmParameterStore>();
		services.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient);
	})
	.Build();

await host.RunAsync();

