using Constructs;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using Tradie.Infrastructure.Resources;

namespace Tradie.Infrastructure.ImageRepository;

public class ImageRepositoryStack : StackBase {
	public readonly EcrRepository ScannerRepository;
	public readonly EcrRepository AnalyzerRepository;
	public readonly EcrRepository ItemLogBuilderRepository;
	
	public ImageRepositoryStack(Construct scope, string id, ResourceConfig config) : base(scope, id, config) {
		this.ScannerRepository = MakeRepo("scanner");
		this.AnalyzerRepository = MakeRepo("analyzer");
		this.ItemLogBuilderRepository = MakeRepo("logbuilder");
	}

	private EcrRepository MakeRepo(string name) {
		return new EcrRepository(this, $"{name}-repo", new EcrRepositoryConfig() {
			Name = $"{name}"
		});
	}
}