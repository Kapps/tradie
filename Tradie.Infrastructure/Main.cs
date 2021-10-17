using System;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;

namespace MyCompany.MyApp {
    class MyApp : TerraformStack {
        const string region = "ca-central-1";
        const string tradieTag = "tradie-ca";
        const string prefix = "tradie-ca";

        public MyApp(Construct scope, string id) : base(scope, id) {
            new AwsProvider(this, "AWS", new AwsProviderConfig {
                Region = region,
                DefaultTags = new[] {
                    new AwsProviderDefaultTags() {
                        Tags = tradieTag,
                    }
                },
            });

            var vpc = new Vpc(this, "tradie", new VpcConfig() {
                CidrBlock = "10.200.0.0/16",
            });

            var ecr = new EcrRepository(this, "scanner", new EcrRepositoryConfig() {
                Name = $"{prefix}-scanner"
            });

            var ecs = new EcsCluster(this, "ecs", new EcsClusterConfig() {
                Name  = $"{prefix}-primary-cluster",
                //CapacityProviders = new string[] { "FARGATE", "EC2" },
            });

			/*var task = new EcsTaskDefinition(this, "scanner-task", new EcsTaskDefinitionConfig() {
				Cpu = "1024",
				Memory = "512",
				NetworkMode = "VPC",
				ExecutionRoleArn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy",
			});*/

			var changeBucket = new S3Bucket(this, "raw-changesets-bucket", new S3BucketConfig() {
				Bucket = "tradie-raw-changesets",
				ForceDestroy = false,
				Versioning = new[] {
					new S3BucketVersioning() {
						Enabled = false,
					}
				},
			});

			var ssm = new SsmParameter(this, "raw-changesets-ssm", new SsmParameterConfig() {
				Name = "Config.ChangeSetBucket",
				Value = changeBucket.Bucket,
				Type = "String",
			});
        }

        public static void Main(string[] args) {
            App app = new App();
            new MyApp(app, "Tradie.Infrastructure");
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}