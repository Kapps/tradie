using System;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.CloudWatch;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using HashiCorp.Cdktf.Providers.Docker;
using System.IO;
using Tradie.Infrastructure.Aspects;
using Tradie.Infrastructure.Resources;

namespace Tradie.Infrastructure {
    public class MyApp : TerraformStack {
	    public MyApp(Construct scope, string id, ResourceConfig config) : base(scope, id) {
	        HashiCorp.Cdktf.Aspects.Of(this).Add(new EnvironmentPrefixerAspect("tradie-ca-dev"));

	        new AwsProvider(this, "AWS", new AwsProviderConfig {
				Region = config.Region,
				DefaultTags = new AwsProviderDefaultTags() {
					Tags = config.Environment,
				}
			});

	        new DockerProvider(this, "docker", new DockerProviderConfig() {
				
	        });

	        var permissions = new Permissions(this);
	        var network = new Tradie.Infrastructure.Resources.Network(this);

            var ecs = new EcsCluster(this, "ecs", new EcsClusterConfig() {
                Name  = $"primary-cluster",
                //CapacityProviders = new string[] { "FARGATE", "EC2" },
            });

            var scanner = new Scanner(this, ecs, config, permissions);
            var analyzer = new Analyzer(this);
	    }

        public static void Main(string[] args) {
            App app = new App();
            var stack = new MyApp(app, "tradie-dev", new ResourceConfig() {
	            Environment = "tradie-dev-ca",
	            Region = "ca-central-1",
	            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../"),
            });
            
            new S3Backend(stack, new S3BackendProps() {
	            Bucket = "tradie-terraform-remote-backend",
	            Region = "us-east-1",
	            Key = "cdktf-remote",
            });
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}