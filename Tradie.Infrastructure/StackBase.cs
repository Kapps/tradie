using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Provider;
using HashiCorp.Cdktf.Providers.Random;
using System.Collections.Generic;
using Tradie.Infrastructure.Aspects;
// ReSharper disable VirtualMemberCallInConstructor

namespace Tradie.Infrastructure;

public class StackBase : TerraformStack {
	public TerraformProvider AwsProvider { get; init; }
	public TerraformProvider AwsUsProvider { get; init; }

	protected StackBase(Construct scope, string id, ResourceConfig config) : base(scope, id) {
		new S3Backend(this, new S3BackendProps() {
			Bucket = "tradie-res-tf-state",
			Region = "ca-central-1",
			Key = $"cdktf-remote-{this.GetType().Name}",
		});

		this._prefixerAspect = new($"tradie-{config.Environment}");
		HashiCorp.Cdktf.Aspects.Of(this).Add(this._prefixerAspect);

		this.AwsProvider = new AwsProvider(this, "AWS", new AwsProviderConfig {
			Region = config.Region,
			DefaultTags = new AwsProviderDefaultTags() {
				Tags = new Dictionary<string, string>() {
					{ "TRADIE_REGION", config.Environment }
				},
			}
		});

		this.AwsUsProvider = new AwsProvider(this, "AWS-USE", new AwsProviderConfig() {
			Region = "us-east-1",
			DefaultTags = new AwsProviderDefaultTags() {
				Tags = new Dictionary<string, string>() {
					{ "TRADIE_REGION", config.Environment }
				}
			},
			Alias = "aws-use"
		});

		//new Providers.Null.NullProvider(this, id);

		_ = new RandomProvider(this, "random-provider", new RandomProviderConfig() {

		});
	}

	protected void ExcludeFromPrefixing(IConstruct resource) {
		this._prefixerAspect.AddExclude(resource);
	}

	private EnvironmentPrefixerAspect _prefixerAspect;
}