using Amazon.SimpleSystemsManagement;
using System;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.CloudWatch;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using HashiCorp.Cdktf.Providers.Docker;
using HashiCorp.Cdktf.Providers.Null;
using HashiCorp.Cdktf.Providers.Random;
using System.IO;
using System.Net;
using System.Net.Http;
using Tradie.Infrastructure.Aspects;
using Tradie.Infrastructure.Resources;

namespace Tradie.Infrastructure {
    public class MyApp : TerraformStack {
	    public MyApp(Construct scope, string id, ResourceConfig config) : base(scope, id) {
		    HashiCorp.Cdktf.Aspects.Of(this).Add(new EnvironmentPrefixerAspect($"tradie-{config.Environment}"));

	        new AwsProvider(this, "AWS", new AwsProviderConfig {
				Region = config.Region,
				DefaultTags = new AwsProviderDefaultTags() {
					Tags = config.Environment,
				}
			});

	        new NullProvider(this, id);

	        new DockerProvider(this, "docker", new DockerProviderConfig() {
				
	        });

	        new RandomProvider(this, "random-provider", new RandomProviderConfig() {

	        });

	        var ssm = new AmazonSimpleSystemsManagementClient();

	        var permissions = new Permissions(this);
	        var network = new Tradie.Infrastructure.Resources.Network(this, config);

	        var rds = new Rds(this, network, config);
		    
	        var ecs = new Ecs(this, network, ssm);
	        var routing = new Routing(this, network, ecs, ssm);

	        var itemStream = new ItemStream(this);

	        var scanner = new Scanner(this, network, ecs, config, permissions);
	        var analyzer = new Analyzer(this, config, permissions, scanner, network, itemStream);
	    }

        public static void Main(string[] args) {
            App app = new App();
            var httpClient = new HttpClient();
            var localIp = IPAddress.Parse(httpClient.GetStringAsync("https://api.ipify.org").Result);
            var devStack = new MyApp(app, "tradie-dev", new ResourceConfig() {
	            Environment = "dev",
	            //Region = "ca-central-1",
	            Region = "us-east-1",
	            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../src/"),
	            Version = "0.1.0",
	            LocalIpAddress = localIp,
            });
            
            new S3Backend(devStack, new S3BackendProps() {
	            Bucket = "tradie-terraform-remote-backend",
	            Region = "us-east-1",
	            Key = "cdktf-remote",
            });
            
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}
