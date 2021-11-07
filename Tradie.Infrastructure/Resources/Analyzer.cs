using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Ssm;

namespace Tradie.Infrastructure.Resources {
	public class Analyzer {
		public readonly S3Bucket analyzedItemBucket;
		public readonly EcrRepository ecrRepo;

		public Analyzer(TerraformStack stack, Construct scope, string id) {
			this.analyzedItemBucket = new S3Bucket(stack, "analyzed-changesets-bucket", new S3BucketConfig() {
				Bucket = "analyzed-changesets",
				ForceDestroy = true,
				Versioning = new S3BucketVersioning() {
					Enabled = false,
				},
			});

			var ssm = new SsmParameter(stack, "analyzed-changesets-ssm", new SsmParameterConfig() {
				Name = "Config.AnalyzedChangeSetBucket",
				Value = this.analyzedItemBucket.Bucket,
				Type = "String",
			});

			this.ecrRepo = new EcrRepository(stack, "analyzer-repo",new EcrRepositoryConfig() {
				Name = "analyzer-repo",
			});
		}
	}
}