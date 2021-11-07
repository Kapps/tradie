using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.CloudWatch;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using System;
namespace Tradie.Infrastructure.Resources {
    public class Scanner {
		public readonly S3Bucket changeBucket;
		public readonly EcrRepository ecrRepo;

        public Scanner(TerraformStack stack, Construct scope, string id) {
	        this.changeBucket = new S3Bucket(stack, "raw-changesets-bucket", new S3BucketConfig() {
				Bucket = "raw-changesets",
				ForceDestroy = true,
				Versioning = new S3BucketVersioning() {
					Enabled = false,
				}
			});	

			var ssm = new SsmParameter(stack, "raw-changesets-ssm", new SsmParameterConfig() {
				Name = "Config.ChangeSetBucket",
				Value = changeBucket.Bucket,
				Type = "String",
			});

			this.ecrRepo = new EcrRepository(stack, "scanner-repo", new EcrRepositoryConfig() {
				Name = $"scanner-repo",
			});

			var logs = new CloudwatchLogGroup(stack, "scanner-log-group", new CloudwatchLogGroupConfig() {
				Name = "scanner-logs",
				RetentionInDays = 14,
			});
		}
    }
}

