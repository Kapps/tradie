using Constructs;
using HashiCorp.Cdktf.Providers.Aws.Servicediscovery;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Packaging;

namespace Tradie.Infrastructure.Indexer;

public class IndexerStack : StackBase {
	public IndexerStack(Construct scope, string id, ResourceConfig config, FoundationStack foundation, PackagedBuild packagedBuild) : base(scope, id, config) {
		var build = packagedBuild.GetPublishedPackage()
			.GetAwaiter().GetResult();

		_ = new SsmParameter(this, "indexer-host-ssm", new SsmParameterConfig() {
			Name = "Config.IndexerGrpcAddress",
			Type = "String",
			Value = "https://srv.tradie.io:5000"
		});

		var cloudMapService = new ServiceDiscoveryService(this, "indexer-discovery", new ServiceDiscoveryServiceConfig() {
			NamespaceId = foundation.CloudMap.CloudMapNamespace.Id,
			Name = "indexer",
			HealthCheckConfig = new ServiceDiscoveryServiceHealthCheckConfig() {
				Type = "HTTPS",
			}
		});

		_ = new SsmParameter(this, "indexer-discovery-ssm", new SsmParameterConfig() {
			Name = "Config.DiscoveryServiceIndexer",
			Type = "String",
			Value = cloudMapService.Id
		});
	}
}