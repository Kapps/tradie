using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Kinesis;
using HashiCorp.Cdktf.Providers.Aws.Ssm;

namespace Tradie.Infrastructure.Resources {
	public class ItemStream {
		public readonly KinesisStream KinesisStream;
		
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
		}
	}
}