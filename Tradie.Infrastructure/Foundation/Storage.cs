using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Ssm;

namespace Tradie.Infrastructure.Foundation;

public class Storage {
	public readonly S3Bucket S3Bucket;

	public Storage(TerraformStack stack) {
		this.S3Bucket = new S3Bucket(stack, "storage-bucket", new S3BucketConfig() {
			Bucket = "storage",
			Lifecycle = new TerraformResourceLifecycle() {
				PreventDestroy = true
			},
			Versioning = new S3BucketVersioning() {
				Enabled = true
			},
		});

		var ssm = new SsmParameter(stack, "storage-bucket-ssm", new SsmParameterConfig() {
			Name = "Config.StorageBucket",
			Type = "String",
			Value = this.S3Bucket.Bucket
		});
	}
}