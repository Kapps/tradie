// See https://aka.ms/new-console-template for more information

using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using Amazon.SimpleSystemsManagement;
using Tradie.ItemLogBuilder;
using Tradie.Common;
using Tradie.TestUtils;

Console.WriteLine("Hello, World!");

var ssm = new AmazonSimpleSystemsManagementClient();
await TradieConfig.InitializeFromEnvironment(ssm);

var context = new TestLambdaContext();
context.RemainingTime = TimeSpan.FromSeconds(3000);
var builderFunc = new Tradie.ItemLogBuilder.Function();
await builderFunc.FunctionHandler(new ScheduledEvent(), context);

/*var analyzerFunc = new Tradie.Analyzer.Function();

await analyzerFunc.FunctionHandler(new S3Event() {
	Records = new List<S3EventNotification.S3EventNotificationRecord>() {
		new() {
			S3 = new() {
				Bucket = new() {
					Name = TradieConfig.ChangeSetBucket,
				},
				Object = new() {
					Key = "raw/1387785418-1392447676-1345885462-1500603708-1446791192.json.br"
				}
			}
		}
	}
}, null);*/