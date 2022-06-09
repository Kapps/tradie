using Constructs;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Packaging;
using Tradie.Infrastructure.Resources;
using Tradie.Infrastructure.Scanner;

namespace Tradie.Infrastructure.Analyzer;

public class AnalyzerStack : StackBase {
	public readonly ItemStream ItemStream;
	public readonly AnalyzerLambda Lambda;
	
	public AnalyzerStack(
		Construct scope, 
		string id, 
		ResourceConfig config,
		FoundationStack foundation,
		ScannerStack scannerStack,
		PackagedBuild packagedBuild
	) : base(scope, id, config) {
		this.ItemStream = new ItemStream(this);
		this.Lambda = new(this, config, foundation.Permissions, scannerStack.Service, foundation.Network,
			this.ItemStream, packagedBuild);
	}
}