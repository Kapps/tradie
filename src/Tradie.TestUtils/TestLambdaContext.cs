using Amazon.Lambda.Core;
using System.Reflection;

namespace Tradie.TestUtils {
	public class TestLambdaContext : ILambdaContext {
		public string AwsRequestId { get; set; } = "Foo";
		public IClientContext? ClientContext { get; set; } = null;
		public string FunctionName { get; set; } = Assembly.GetEntryAssembly()!.GetName().Name!;
		public string FunctionVersion { get; set; } = Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
		public ICognitoIdentity? Identity { get; set; } = null;
		public string? InvokedFunctionArn { get; set; } = null;
		public ILambdaLogger? Logger { get; set; } = null;
		public string? LogGroupName { get; set; } = null;
		public string? LogStreamName { get; set; } = null;
		public int MemoryLimitInMB { get; set; } = 2048;
		public TimeSpan RemainingTime { get; set; } = TimeSpan.FromMinutes(1200);
	}
}