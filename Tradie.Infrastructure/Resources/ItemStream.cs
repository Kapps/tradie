using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Kinesis;
using HashiCorp.Cdktf.Providers.Aws.Sqs;
using HashiCorp.Cdktf.Providers.Aws.Ssm;

namespace Tradie.Infrastructure.Resources {
	public class ItemStream {
		public readonly KinesisStream KinesisStream;
		public readonly SqsQueue PoisonPillQueue;
		
		public ItemStream(TerraformStack stack) {
			this.KinesisStream = new KinesisStream(stack, "item-stream", new KinesisStreamConfig() {
				ShardCount = 1,
				Name = "item-stream",
			});

			var streamSsmParam = new SsmParameter(stack, "item-stream-ssm", new SsmParameterConfig() {
				Name = "Config.AnalyzedItemStreamName",
				Type = "String",
				Value = this.KinesisStream.Name,
			});

			this.PoisonPillQueue = new SqsQueue(stack, "item-stream-ppq", new SqsQueueConfig() {
				Name = "item-stream-ppq",
			});

			var ppqSsmParam = new SsmParameter(stack, "item-stream-ppq-ssm", new SsmParameterConfig() {
				Name = "Config.ItemStreamPoisonPillQueueUrl",
				Type = "String",
				Value = this.PoisonPillQueue.Url
			});
		}
	}
}