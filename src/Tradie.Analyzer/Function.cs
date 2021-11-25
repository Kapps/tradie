using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tradie.Analyzer.Analysis;
using Tradie.Analyzer.Analysis.Analyzers;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Tradie.Analyzer; 


public class Function {
	public async Task FunctionHandler(S3Event input, ILambdaContext context) {
		var ssmClient = new AmazonSimpleSystemsManagementClient();
		var s3Client = new AmazonS3Client();

		await TradieConfig.InitializeFromEnvironment(ssmClient);
		
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
					.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
					.AddSingleton<IAmazonS3>(s3Client)
					.AddSingleton<IItemAnalyzer, ItemTypeAnalyzer>()
					.AddSingleton<IItemTypeRepository, ItemTypeRepository>();
			})
			.Build();
		
		Console.WriteLine($"Got input file {input.Records[0].S3.Object.Key}.");

		var itemAnalyzers = host.Services.GetServices<IItemAnalyzer>().ToArray();

		foreach(var analyzer in itemAnalyzers) {
			Console.WriteLine($"Registered analyzer {analyzer.GetType().Name}.");
		}
		
		var stashAnalyzer = new StashTabAnalyzer(itemAnalyzers);
		
		foreach(var record in input.Records) {
			var obj = await s3Client.GetObjectAsync(record.S3.Bucket.Name, record.S3.Object.Key);
			await using var jsonStream = obj.ResponseStream;

			var stashes = await SpanJson.JsonSerializer.Generic.Utf8.DeserializeAsync<RawStashTab[]>(jsonStream);
			foreach(var stash in stashes) {
				Console.WriteLine($"Found stash with id {stash.Id} containing {stash.Items.Length} items.");
				var analyzedItems = await stashAnalyzer.AnalyzeTab(stash);
				
			}
		}
	}
}