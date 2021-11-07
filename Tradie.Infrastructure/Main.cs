using System;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.CloudWatch;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using Tradie.Infrastructure.Aspects;
using Tradie.Infrastructure.Resources;

namespace Tradie.Infrastructure {
    public class MyApp : TerraformStack {
        const string region = "ca-central-1";
        const string tradieTag = "tradie-ca";
        const string prefix = "tradie-ca";

        public MyApp(Construct scope, string id) : base(scope, id) {
	        HashiCorp.Cdktf.Aspects.Of(this).Add(new EnvironmentPrefixerAspect("tradie-ca-dev"));

	        new S3Backend(this, new S3BackendProps() {
				Bucket = "tradie-terraform-remote-backend",
		        Region = "us-east-1",
		        Key = "cdktf-remote",
	        });

			new AwsProvider(this, "AWS", new AwsProviderConfig {
				Region = region,
				DefaultTags = new AwsProviderDefaultTags() {
					Tags = tradieTag,
				}
			});

            var vpc = new Vpc(this, "tradie", new VpcConfig() {
                CidrBlock = "10.200.0.0/16",
            });

            var ecs = new EcsCluster(this, "ecs", new EcsClusterConfig() {
                Name  = $"primary-cluster",
                //CapacityProviders = new string[] { "FARGATE", "EC2" },
            });

            var scanner = new Scanner(this, scope, id);
            var analyzer = new Analyzer(this, scope, id);

            /*var task = new EcsTaskDefinition(this, "scanner-task", new EcsTaskDefinitionConfig() {
	            Cpu = "1024",
	            Memory = "512",
	            NetworkMode = "VPC",
	            ExecutionRoleArn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy",
            });*/


        }

        public static void Main(string[] args) {
            App app = new App();
            new MyApp(app, "Tradie.Infrastructure");
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}