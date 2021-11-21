using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.SimpleSystemsManagement;
using Tradie.Common;

namespace Tradie.Analyzer; 

public class Function {
	public async Task FunctionHandler(S3Event input, ILambdaContext context) {
		var ssm = new AmazonSimpleSystemsManagementClient();
		await TradieConfig.InitializeFromEnvironment(ssm);
		Console.WriteLine($"Got input file  {input.Records[0].S3.Object.Key}.");
	}
}