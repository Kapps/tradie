using System.Threading.Tasks;

namespace Tradie.Infrastructure.Packaging;

public class PackagedBuild {
	public bool IsDirty { get; set; }

	public PackagedBuild(DockerPackager packager, string dockerPath, string repoName, string platform) {
		this._repoName = repoName;
		this._packager = packager;
		this._dockerPath = dockerPath;
		this._platform = platform;
	}

	/*public async Task UpdatePackage() {
		this._compiledImage = await this._packager.BuildAndPushProject(this._dockerPath, this._repoName);
	}*/

	public async Task<EcrImage> GetPublishedPackage() {
		return this._compiledImage ??= await (
			this.IsDirty
				? this._packager.BuildAndPushProject(this._dockerPath, this._repoName, this._platform)
				: this._packager.GetExistingImageId(this._repoName)
		);
	}

	private EcrImage _compiledImage;
	private DockerPackager _packager;
	private string _repoName;
	private string _dockerPath;
	private string _platform;
}