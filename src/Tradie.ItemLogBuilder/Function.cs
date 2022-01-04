using Amazon.Kinesis;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tradie.Common;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Tradie.ItemLogBuilder; 


public class Function {
	public async Task FunctionHandler(ScheduledEvent input, ILambdaContext context) {
		try {
			var ssmClient = new AmazonSimpleSystemsManagementClient();
			var s3Client = new AmazonS3Client();

			await TradieConfig.InitializeFromEnvironment(ssmClient);
			
			Console.WriteLine($"Starting ItemLog with build hash {Environment.GetEnvironmentVariable("BUILD_HASH")}");
			
			IHost host = Host.CreateDefaultBuilder()
				.ConfigureServices(services => {
					services.AddLogging(builder => {
						builder.AddSimpleConsole(format => {
							format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
							format.UseUtcTimestamp = true;
							format.IncludeScopes = true;
							format.SingleLine = false;
						});
					});
					services.AddSingleton<IParameterStore, SsmParameterStore>()
						.AddSingleton<IAmazonKinesis, AmazonKinesisClient>()
						.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
						.AddSingleton<IAmazonS3>(s3Client);
						
				})
				.Build();


			
		} catch(Exception ex) {
			Console.WriteLine("---Exception Details---");
			Console.WriteLine(ex.ToString());
			throw;
		}
	}
}