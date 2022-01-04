// See https://aka.ms/new-console-template for more information

using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using Amazon.SimpleSystemsManagement;
using Tradie.Analyzer;
using Tradie.Common;

Console.WriteLine("Hello, World!");

var ssm = new AmazonSimpleSystemsManagementClient();
await TradieConfig.InitializeFromEnvironment(ssm);

var analyzerFunc = new Function();

await analyzerFunc.FunctionHandler(new S3Event() {
	Records = new List<S3EventNotification.S3EventNotificationRecord>() {
		new() {
			S3 = new() {
				Bucket = new() {
					Name = TradieConfig.ChangeSetBucket,
				},
				Object = new() {
					Key = "raw/1362191264-1367618240-1321538123-1474361838-1420646476.json.br"
				}
			}
		}
	}
}, null);