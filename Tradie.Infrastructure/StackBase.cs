using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Random;
using System.Collections.Generic;
using Tradie.Infrastructure.Aspects;

namespace Tradie.Infrastructure;

public class StackBase : TerraformStack {
	public StackBase(Construct scope, string id, ResourceConfig config) : base(scope, id) {
		new S3Backend(this, new S3BackendProps() {
			Bucket = "tradie-res-tf-state",
			Region = "ca-central-1",
			Key = $"cdktf-remote-{this.GetType().Name}",
		});
		
		HashiCorp.Cdktf.Aspects.Of(this).Add(new EnvironmentPrefixerAspect($"tradie-{config.Environment}"));

		new AwsProvider(this, "AWS", new AwsProviderConfig {
			Region = config.Region,
			DefaultTags = new AwsProviderDefaultTags() {
				Tags = new Dictionary<string, string>() {
					{ "TRADIE_REGION", config.Environment }
				},
			}
		});

		new Providers.Null.NullProvider(this, id);

		new RandomProvider(this, "random-provider", new RandomProviderConfig() {

		});
	}
}