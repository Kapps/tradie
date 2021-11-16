using Amazon.SimpleSystemsManagement;
using Tradie.Analyzer;
using Tradie.Common;

string environment = System.Environment.GetEnvironmentVariable("TRADIE_ENV")
                     ?? throw new ArgumentNullException("TRADIE_ENV");

var ssm = new AmazonSimpleSystemsManagementClient();

await TradieConfig.InitializeFromSsm(environment, ssm);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

