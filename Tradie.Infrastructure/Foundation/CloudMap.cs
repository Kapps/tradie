using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.ServiceDiscoveryHttpNamespace;
using HashiCorp.Cdktf.Providers.Aws.SsmParameter;

namespace Tradie.Infrastructure.Foundation;

public class CloudMap {
	public readonly ServiceDiscoveryHttpNamespace CloudMapNamespace;
	
	public CloudMap(TerraformStack stack) {
		this.CloudMapNamespace = new ServiceDiscoveryHttpNamespace(stack, "cloudmap", new ServiceDiscoveryHttpNamespaceConfig() {
			Name = "cloud-map",
		});
		
		_ = new SsmParameter(stack, "cloudmap-ssm", new SsmParameterConfig() {
			Name = "Config.DiscoveryNamespaceId",
			Type = "String",
			Value = this.CloudMapNamespace.Id
		});
		
		_ = new SsmParameter(stack, "cloudmap-name-ssm", new SsmParameterConfig() {
			Name = "Config.DiscoveryNamespaceName",
			Type = "String",
			Value = this.CloudMapNamespace.Name
		});
	}
}