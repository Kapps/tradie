using Amazon.SimpleSystemsManagement;
using Tradie.Analyzer;
using Tradie.Common;



var ssm = new AmazonSimpleSystemsManagementClient();
await TradieConfig.InitializeFromEnvironment(ssm);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

