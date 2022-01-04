using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using System.IO;
using System.Threading;

namespace Tradie.Infrastructure.Resources {
	/// <summary>
	/// Terraform resource that creates an ECR repo and builds a dotnet project, publishing to it.
	/// </summary>
	public class EcrProjectRepository {
		/// <summary>
		/// The ECR repository created for these builds.
		/// </summary>
		public readonly EcrRepository EcrRepo;
		/// <summary>
		/// The tag for the build version that was pushed to the repo.
		/// </summary>
		public readonly string HashTag;
		/// <summary>
		/// The tag for the build version that is the latest build.
		/// </summary>
		public readonly string LatestTag;
		/// <summary>
		/// The resource that handles the building of this project.
		/// Anything that needs the compiled output of the project should depend on this resource.
		/// </summary>
		public readonly HashiCorp.Cdktf.Providers.Null.Resource BuildResource;
		/// <summary>
		/// EcrImage with an ID of the build version.
		/// </summary>
		public readonly DataAwsEcrImage EcrImage;
		/// <summary>
		/// Full URL to the EcrImage for this build version (latest tag).
		/// </summary>
		public string EcrImageUri => $"{this.EcrRepo.RepositoryUrl}@{this.EcrImage.Id}";

		/// <summary>
		/// Creates a new DotnetDockerRepository that builds the given project and pushes it to an ECR repo.
		/// </summary>
		/// <param name="stack">Terraform stack.</param>
		/// <param name="name">Short-form name of the project, used in resource names.</param>
		/// <param name="projectFolder">The relative path to the folder from the `src` folder, excluding leading and trailing slashes.</param>
		/// <param name="resourceConfig">Configuration settings</param>
		public EcrProjectRepository(TerraformStack stack, string name, string projectFolder, ResourceConfig resourceConfig) {
			this.EcrRepo = new EcrRepository(stack, $"{name}-repo", new EcrRepositoryConfig() {
				Name = $"{name}-repo",
			});

			var auth = new DataAwsEcrAuthorizationToken(stack, $"{name}-auth", new DataAwsEcrAuthorizationTokenConfig() {
				DependsOn = new ITerraformDependable[] {this.EcrRepo},
				RegistryId = this.EcrRepo.RegistryId,
			});

			if(_cachedSolutionAsset == null) {
				lock(CachedSolutionLock) {
					if(_cachedSolutionAsset == null) {
						_cachedSolutionAsset = new TerraformAsset(stack, $"solution-directory", new TerraformAssetConfig() {
							Path = Path.Combine(resourceConfig.BaseDirectory, "./"),
						});
					}
				}
			}

			this.HashTag = $"{this.EcrRepo.RepositoryUrl}:{resourceConfig.Version}-{_cachedSolutionAsset.AssetHash}";
			this.LatestTag = $"{this.EcrRepo.RepositoryUrl}:latest";
			
			this.BuildResource = new HashiCorp.Cdktf.Providers.Null.Resource(stack, $"{name}-image-{this.HashTag}", new HashiCorp.Cdktf.Providers.Null.ResourceConfig() {
				DependsOn = new ITerraformDependable[] { auth },
			});
			this.BuildResource.AddOverride("provisioner.local-exec.command",
				$"docker login -u \"{auth.UserName}\" -p \"{auth.Password}\" \"{auth.ProxyEndpoint}\" && "
				+ $"docker buildx build -f \"{resourceConfig.BaseDirectory}/{projectFolder}/Dockerfile\" -t \"{this.HashTag}\" -t \"{this.LatestTag}\" \"{_cachedSolutionAsset.Path}\" --platform linux/arm64 && " 
				+ $"docker push \"{this.LatestTag}\"");

			this.EcrImage = new DataAwsEcrImage(stack, $"{name}-ecr-image", new DataAwsEcrImageConfig() {
				ImageTag = "latest",
				RepositoryName = this.EcrRepo.Name,
			});
		}

		private static TerraformAsset _cachedSolutionAsset;
		private static readonly Mutex CachedSolutionLock = new Mutex();
	}
}