using Amazon.ECR;
using Amazon.ECR.Model;
using Docker.DotNet;
using Docker.DotNet.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tradie.Infrastructure.Packaging;

/// <summary>
/// Result of a built docker image pushed to an ECR repository.
/// </summary>
public record EcrImage(
	string TaggedImageUri,
	ImageIdentifier ImageId
);

public class DockerPackager {
	private static HashSet<string> excludedFolders = new(StringComparer.InvariantCultureIgnoreCase) {
		"bin", "obj", ".git", "Tradie.Client",
		".idea", ".vs", "TestResults", "node_modules", 
	};
	
	public DockerPackager(ResourceConfig config) {
		this._config = config;
		this._dockerClient = new DockerClientConfiguration().CreateClient();
		this._contextLock = new(1);
		this._ecrClient = new AmazonECRClient();
	}

	private async Task<Repository> GetRepo(string repoName) {
		try {
			var repoResp = await this._ecrClient.DescribeRepositoriesAsync(new() {
				RepositoryNames = new() {$"tradie-{this._config.Environment}-{repoName}"}
			});
			var repo = repoResp.Repositories.Single();
			return repo;
		} catch(RepositoryNotFoundException) {
			return null;
		}
	}

	public async Task<EcrImage> GetExistingImageId(string repoName) {
		var repo = await GetRepo(repoName);
		if(repo == null) {
			// No repo built yet, so nothing to deploy yet.
			Console.WriteLine($"No repo for {repoName} found -- returning empty image.");
			return new EcrImage("", new ImageIdentifier() { ImageDigest = "", ImageTag = "latest" });
		}

		try {
			var resp = await this._ecrClient.DescribeImagesAsync(new() {
				RepositoryName = repo.RepositoryName,
				ImageIds = new() {
					new() {
						ImageTag = "latest"
					}
				}
			});

			var image = resp.ImageDetails.Single();
			string hash = image.ImageTags.First(c => c != "latest");

			return new EcrImage(
				$"{repo.RepositoryUri}:{hash}",
				new() {ImageDigest = image.ImageDigest, ImageTag = hash}
			);
		} catch(ImageNotFoundException) {
			Console.WriteLine($"No existing image for {repoName} found -- returning empty image.");
			return new EcrImage("", new ImageIdentifier() { ImageDigest = "", ImageTag = "latest" });
		}
	}

	public async Task<EcrImage> BuildAndPushProject(string dockerPath, string repoName, string platform) {
		Console.WriteLine($"Preparing new build of {repoName} repository");
		await this._contextLock.WaitAsync();
		try {
			if(!this._contextCreated) {
				Console.WriteLine("Creating docker context");
				await this.CreateDockerContext();
				Console.WriteLine("Done creating docker context");
				this._contextCreated = true;
			}
		} finally {
			this._contextLock.Release();
		}

		var repo = await GetRepo(repoName);

		var auth = await GetAuthConfig(repo);

		await using var fs = File.OpenRead(this.GetPathForContext());
		Console.WriteLine($"Building image {repo.RepositoryName} from {dockerPath}");

		var build = new DockerBuild();

		string latestTag = $"{repo.RepositoryUri}:latest";
		string hash = DateTime.Now.ToFileTime().ToString();
		string hashTag = $"{repo.RepositoryUri}:{hash}";
		await this._dockerClient.Images.BuildImageFromDockerfileAsync(new ImageBuildParameters() {
			Dockerfile = dockerPath,
			Platform = platform,
			Memory = 3L * 1024 * 1024 * 1024,
			MemorySwap = 4L * 1024 * 1024 * 1024,
			// NoCache = true,
			BuildArgs = new Dictionary<string, string>() {
				{ "Platform", platform }
			},
			Labels = new Dictionary<string, string>() {
				{ "Platform", platform }
			},
			//Platform = "linux/amd64,linux/arm64",
			Tags = new[] {latestTag, hashTag},
		}, fs, new[] {auth}, null, build.CreateBuildProgressAction(), CancellationToken.None);

		build.ThrowIfError();
		Console.WriteLine("Pushing repo");

		await this._dockerClient.Images.PushImageAsync(hashTag, new ImagePushParameters() {
			Tag = "latest",
		}, auth, build.CreateProgressAction(), CancellationToken.None);

		build.ThrowIfError();
		Console.WriteLine("Done pushing latest tag to repo.");

		var image = await this._ecrClient.BatchGetImageAsync(new() {
			RepositoryName = repo.RepositoryName,
			ImageIds = new() {new ImageIdentifier() {ImageTag = "latest"}}
		});

		var resp = await this._ecrClient.PutImageAsync(new() {
			ImageTag = hash,
			ImageDigest = image.Images.Single().ImageId.ImageDigest,
			ImageManifest = image.Images.Single().ImageManifest,
			RepositoryName = repo.RepositoryName,
			ImageManifestMediaType = image.Images.Single().ImageManifestMediaType
		});
		Console.WriteLine($"Done retagging to add tag {hash}.");

		return new(
			hashTag,
			new() {ImageDigest = resp.Image.ImageId.ImageDigest, ImageTag = hash}
		);
	}

	private async Task<AuthConfig> GetAuthConfig(Repository repo) {
		var authResp = await this._ecrClient.GetAuthorizationTokenAsync(new GetAuthorizationTokenRequest());
		string authToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authResp.AuthorizationData[0].AuthorizationToken));
		string[] authSplit = authToken.Split(':');
		string username = authSplit[0];
		string password = authSplit[1];

		return new() {
			Auth = authToken,
			ServerAddress = repo.RepositoryUri,
			Username = username,
			Password = password,
		};
	}

	private async Task CreateDockerContext() {
		if(this._contextCreated) {
			return;
		}

		this._contextCreated = true;
		string sourceDir = this.GetSourceDirectory();
		string contextPath = this.GetPathForContext();
		Directory.CreateDirectory(Path.GetDirectoryName(contextPath)!);
		
		await using var fs = File.Create(this.GetPathForContext());
		using var archive = SharpCompress.Archives.Tar.TarArchive.Create();
		
		foreach(var filePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories)) {
			string key = Path.GetRelativePath(sourceDir, filePath);
			var parts = key.Split(Path.DirectorySeparatorChar);
			if(parts.Any(c => excludedFolders.Contains(c))) {
				//Console.WriteLine($"Skipping file {key}");
				continue;
			}
			
			//Console.WriteLine($"Adding file {key}");
			archive.AddEntry(key, filePath);
		}
		
		archive.SaveTo(fs, new WriterOptions(CompressionType.None));
	}
	
	private string GetPathForContext() {
		return Path.Combine(Path.GetTempPath(), "tradie/buildContext.tar.gz");
		//return Path.GetFullPath(Path.Combine(this._config.BaseDirectory, "../.buildContext/context.tar"));
	}

	private string GetSourceDirectory() {
		return Path.Combine(this._config.BaseDirectory);
	}

	private ResourceConfig _config;
	private bool _contextCreated;
	private SemaphoreSlim _contextLock;
	private DockerClient _dockerClient;
	private AmazonECRClient _ecrClient;
}