using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Servicediscovery;
using HashiCorp.Cdktf.Providers.Aws.Ssm;

namespace Tradie.Infrastructure.Foundation;

public class CloudMap {
	public readonly ServiceDiscoveryHttpNamespace CloudMapNamespace;
	
	public CloudMap(TerraformStack stack) {
		this.CloudMapNamespace = new ServiceDiscoveryHttpNamespace(stack, "cloudmap", new ServiceDiscoveryHttpNamespaceConfig() {
			Name = "cloud-map",
		});
		
		_ = new SsmParameter(stack, "cloudmap-ssm", new SsmParameterConfig() {
			Name = "Config.DiscoveryNamespace",
			Type = "String",
			Value = this.CloudMapNamespace.Id
		});
	}
}