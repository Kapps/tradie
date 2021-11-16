using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.CloudWatch;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Iam;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using HashiCorp.Cdktf.Providers.Null;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Tradie.Infrastructure.Resources {
    public class Scanner {
		public readonly S3Bucket changeBucket;
		public readonly EcrRepository ecrRepo;

        public Scanner(
	        TerraformStack stack,
	        EcsCluster ecsCluster,
	        ResourceConfig resourceConfig,
	        Permissions permissions
	    ) {
	        this.changeBucket = new S3Bucket(stack, "raw-changesets-bucket", new S3BucketConfig() {
				Bucket = "raw-changesets",
				ForceDestroy = true,
				Versioning = new S3BucketVersioning() {
					Enabled = false,
				}
			});	

			new SsmParameter(stack, "raw-changesets-ssm", new SsmParameterConfig() {
				Name = "Config.ChangeSetBucket",
				Value = changeBucket.Bucket,
				Type = "String",
			});

			this.ecrRepo = new EcrRepository(stack, "scanner-repo", new EcrRepositoryConfig() {
				Name = $"scanner-repo",
			});

			var auth = new DataAwsEcrAuthorizationToken(stack, "scanner-auth", new DataAwsEcrAuthorizationTokenConfig() {
				DependsOn = new [] {this.ecrRepo},
				RegistryId = this.ecrRepo.RegistryId,
			});
			
			

			var asset = new TerraformAsset(stack, "scanner-project", new TerraformAssetConfig() {
				Path = Path.Combine(resourceConfig.BaseDirectory, "./"),
			});

			string tag = $"{this.ecrRepo.RepositoryUrl}:{resourceConfig.Version}-{asset.AssetHash}";

			var image = new HashiCorp.Cdktf.Providers.Null.Resource(stack, $"scanner-image-{tag}", new HashiCorp.Cdktf.Providers.Null.ResourceConfig() {
				DependsOn = new[] { auth },
			});
			image.AddOverride("provisioner.local-exec.command",
				$"docker login -u \"{auth.UserName}\" -p \"{auth.Password}\" \"{auth.ProxyEndpoint}\" && "
				+ $"docker build -f \"{resourceConfig.BaseDirectory}/Tradie.Scanner/Dockerfile\" -t \"{tag}\" \"{asset.Path}\" && " 
				+ $"docker push \"{tag}\"");

			var logs = new CloudwatchLogGroup(stack, "scanner-log-group", new CloudwatchLogGroupConfig() {
				Name = "scanner-logs",
				RetentionInDays = 14,
			});

			var taskRole = new IamRole(stack, "scanner-task-role", new IamRoleConfig() {
				Name = "scanner-task-role",
				InlinePolicy = new[] {
					permissions.InlineLogPolicy,
					permissions.ReadConfigPolicy,
					new IamRoleInlinePolicy() {
						Name = "allow-s3-readwrite",
						Policy = JsonSerializer.Serialize(new {
							Version = Permissions.PolicyVersion,
							Statement = new[] {
								new {
									Effect = "Allow",
									Action = new[] {
										"s3:AbortMultipartUpload",
										"s3:CompleteMultipartUpload",
										"s3:CreateMultipartUpload",
										"s3:GetObject",
										"s3:HeadObject",
										"s3:ListBuckets",
										"s3:ListObjects",
										"s3:ListMultipartUploads",
										"s3:UploadPart",
									},
									Resource = new[] {
										this.changeBucket.Arn,
									}
								}
							},
						})
					}
				},
				AssumeRolePolicy = Permissions.AssumeRolePolicy,
			});
			
			new EcsTaskDefinition(stack, "scanner-taskdef", new EcsTaskDefinitionConfig() {
				Cpu = "256",
				Memory = "512",
				NetworkMode = "awsvpc",
				Family = "scanner",
				TaskRoleArn = taskRole.Arn,
				RequiresCompatibilities = new [] { "EC2", "FARGATE" },
				ExecutionRoleArn = permissions.ExecutionRole.Arn,
				DependsOn = new[] { image },
				ContainerDefinitions = JsonSerializer.Serialize(new[] {
					new {
						name = "tradie-scanner",
						tag,
						image = tag,
						cpu = 256,
						memory = 512,
						executionRoleArn = permissions.ExecutionRole.Arn,
						taskRole = taskRole.Arn,
						environment = new[] {
							new {
								name = "TRADIE_ENV",
								Value = resourceConfig.Environment,
							},
						},
						logConfiguration = new {
							logDriver = "awslogs",
							options = new Dictionary<string, string>() { // Invalid identifiers used, so use a Dictionary.
								{ "awslogs-group", logs.Name },
								{ "awslogs-region", resourceConfig.Region },
								{ "awslogs-stream-prefix", "tradie-scanner" }, 
							}
						},
					},
				}),
			});
        }
    }
}

