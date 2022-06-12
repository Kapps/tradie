using Constructs;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Packaging;
using Tradie.Infrastructure.Resources;
using Tradie.Infrastructure.Scanner;

namespace Tradie.Infrastructure.Web;

public class WebStack : StackBase {
	public readonly WebLambda Lambda;
	
	public WebStack(
		Construct scope, 
		string id, 
		ResourceConfig config,
		FoundationStack foundation,
		PackagedBuild packagedBuild
	) : base(scope, id, config) {
		this.Lambda = new(this, config, foundation, packagedBuild);
	}
}