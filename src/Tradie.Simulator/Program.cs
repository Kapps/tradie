// See https://aka.ms/new-console-template for more information

using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using Amazon.SimpleSystemsManagement;
using Tradie.Common;
using Tradie.Simulator.Scripts;
using Tradie.TestUtils;

var ssm = new AmazonSimpleSystemsManagementClient();
await TradieConfig.InitializeFromEnvironment(ssm);

bool analyzer = false;

if(analyzer) {
	var analyzerFunc = new Tradie.Analyzer.Function();
	await analyzerFunc.FunctionHandler(new S3Event() {
		Records = new List<S3EventNotification.S3EventNotificationRecord>() {
			new() {
				S3 = new() {
					Bucket = new() {
						Name = TradieConfig.ChangeSetBucket,
					},
					Object = new() {
						Key = "raw/1297364325-1302911593-1258544875-1406014261-1352585732.json.br"
					}
				}
			}
		}
	}, new TestLambdaContext());
}

ScriptBase scriptToRun;

scriptToRun = new MigrateToItemsScript();

await scriptToRun.Run();