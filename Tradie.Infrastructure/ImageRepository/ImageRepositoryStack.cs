using Constructs;
using HashiCorp.Cdktf.Providers.Aws.EcrRepository;

namespace Tradie.Infrastructure.ImageRepository;

public class ImageRepositoryStack : StackBase {
	public readonly EcrRepository ScannerRepository;
	public readonly EcrRepository AnalyzerRepository;
	public readonly EcrRepository ItemLogBuilderRepository;
	public readonly EcrRepository WebRepository;
	public readonly EcrRepository IndexerRepository;
	
	public ImageRepositoryStack(Construct scope, string id, ResourceConfig config) : base(scope, id, config) {
		this.ScannerRepository = MakeRepo("scanner");
		this.AnalyzerRepository = MakeRepo("analyzer");
		this.ItemLogBuilderRepository = MakeRepo("logbuilder");
		this.WebRepository = MakeRepo("web");
		this.IndexerRepository = MakeRepo("indexer");
	}

	private EcrRepository MakeRepo(string name) {
		return new EcrRepository(this, $"{name}-repo", new EcrRepositoryConfig() {
			Name = $"{name}"
		});
	}
}