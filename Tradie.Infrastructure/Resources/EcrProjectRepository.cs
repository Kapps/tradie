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
		public readonly string Tag;
		/// <summary>
		/// The resource that handles the building of this project.
		/// Anything that needs the compiled output of the project should depend on this resource.
		/// </summary>
		public readonly HashiCorp.Cdktf.Providers.Null.Resource BuildResource;

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
				lock(_cachedSolutionLock) {
					if(_cachedSolutionAsset == null) {
						_cachedSolutionAsset = new TerraformAsset(stack, $"solution-directory", new TerraformAssetConfig() {
							Path = Path.Combine(resourceConfig.BaseDirectory, "./"),
						});
					}
				}
			}

			this.Tag = $"{this.EcrRepo.RepositoryUrl}:{resourceConfig.Version}-{_cachedSolutionAsset.AssetHash}";

			this.BuildResource = new HashiCorp.Cdktf.Providers.Null.Resource(stack, $"{name}-image-{this.Tag}", new HashiCorp.Cdktf.Providers.Null.ResourceConfig() {
				DependsOn = new ITerraformDependable[] { auth },
			});
			this.BuildResource.AddOverride("provisioner.local-exec.command",
				$"docker login -u \"{auth.UserName}\" -p \"{auth.Password}\" \"{auth.ProxyEndpoint}\" && "
				+ $"docker buildx build -f \"{resourceConfig.BaseDirectory}/{projectFolder}/Dockerfile\" -t \"{this.Tag}\" \"{_cachedSolutionAsset.Path}\" --platform linux/arm64 && " 
				+ $"docker push \"{this.Tag}\"");
		}

		private static TerraformAsset _cachedSolutionAsset;
		private static readonly Mutex _cachedSolutionLock = new Mutex();
	}
}