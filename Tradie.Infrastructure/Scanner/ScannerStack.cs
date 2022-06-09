using Constructs;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.ImageRepository;
using Tradie.Infrastructure.Packaging;

namespace Tradie.Infrastructure.Scanner;

public class ScannerStack: StackBase {
	public readonly ScannerService Service;
	
	public ScannerStack(
		Construct scope, 
		string id, 
		ResourceConfig config,
		FoundationStack foundation,
		PackagedBuild build
	) : base(scope, id, config) {
		
		this.Service = new(this, foundation.EcsCluster, config, foundation.Permissions, build);
	}
}