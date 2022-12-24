using Constructs;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Packaging;

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