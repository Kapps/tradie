using Constructs;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using Tradie.Infrastructure.Packaging;

namespace Tradie.Infrastructure.Indexer;

public class IndexerStack : StackBase {
	public IndexerStack(Construct scope, string id, ResourceConfig config, PackagedBuild packagedBuild) : base(scope, id, config) {
		var build = packagedBuild.GetPublishedPackage()
			.GetAwaiter().GetResult();

		var indexerHostParam = new SsmParameter(this, "indexer-host-ssm", new SsmParameterConfig() {
			Name = "Config.IndexerGrpcAddress",
			Type = "String",
			Value = "https://srv.tradie.io:5000"
		});
	}
}