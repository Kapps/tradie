using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Cloudwatch;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Iam;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using System.Collections.Generic;
using System.Text.Json;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Packaging;
using Tradie.Infrastructure.Resources;

namespace Tradie.Infrastructure.Scanner;

public class ScannerService {
	public readonly S3Bucket ChangeBucket;

	public ScannerService(
		TerraformStack stack,
		Ecs ecs,
		ResourceConfig resourceConfig,
		Permissions permissions,
		PackagedBuild packagedBuild
	) {
		var build = packagedBuild.GetPublishedPackage()
			.GetAwaiter().GetResult();

		this.ChangeBucket = new S3Bucket(stack, "raw-changesets-bucket", new S3BucketConfig() {
			Bucket = "raw-changesets",
			ForceDestroy = true,
			Versioning = new S3BucketVersioning() {
				Enabled = false,
			},
			LifecycleRule = new IS3BucketLifecycleRule[] {
				new S3BucketLifecycleRule() {
					Enabled = true,
					Expiration = new S3BucketLifecycleRuleExpiration() {
						Days = 30
					},
					AbortIncompleteMultipartUploadDays = 7,
					NoncurrentVersionExpiration = new S3BucketLifecycleRuleNoncurrentVersionExpiration() {
						Days = 30
					}
				},
			}
		});

		_ = new SsmParameter(stack, "raw-changesets-ssm", new SsmParameterConfig() {
			Name = "Config.ChangeSetBucket",
			Value = this.ChangeBucket.Bucket,
			Type = "String",
		});

		var logs = new CloudwatchLogGroup(stack, "scanner-log-group", new CloudwatchLogGroupConfig() {
			Name = "scanner-logs",
			RetentionInDays = 14,
		});

		var taskRole = new IamRole(stack, "scanner-task-role", new IamRoleConfig() {
			Name = "scanner-task-role",
			InlinePolicy = new[] {
				permissions.AllowLoggingPolicy,
				permissions.ReadConfigPolicy,
				new IamRoleInlinePolicy() {
					Name = "allow-s3-readwrite",
					Policy = JsonSerializer.Serialize(new {
						Version = Permissions.PolicyVersion,
						Statement = new[] {
							new {
								Effect = "Allow",
								Action = new[] {"s3:*"},
								Resource = new[] {
									this.ChangeBucket.Arn,
									$"{this.ChangeBucket.Arn}/*",
								}
							}
						},
					})
				},
				new IamRoleInlinePolicy() {
					Name = "allow-ssm-write",
					Policy = JsonSerializer.Serialize(new {
						Version = Permissions.PolicyVersion,
						Statement = new[] {
							new {
								Effect = "Allow",
								Action = new[] {"ssm:PutParameter"},
								Resource = new[] {"*"},
							}
						}
					}),
				}
			},
			AssumeRolePolicy = Permissions.EcsAssumeRolePolicy,
		});

		var taskDef = new EcsTaskDefinition(stack, "scanner-taskdef", new EcsTaskDefinitionConfig() {
			Cpu = "512",
			Memory = "512",
			NetworkMode = "host",
			Family = "scanner",
			TaskRoleArn = taskRole.Arn,
			RequiresCompatibilities = new[] {"EC2"},
			ExecutionRoleArn = permissions.ExecutionRole.Arn,
			//DependsOn = new[] {this.Repo.BuildResource},
			ContainerDefinitions = JsonSerializer.Serialize(new[] {
				new {
					name = "tradie-scanner",
					tag = build.ImageId.ImageTag,
					image = build.TaggedImageUri,
					cpu = 512,
					memory = 512,
					executionRoleArn = permissions.ExecutionRole.Arn,
					taskRole = taskRole.Arn,
					environment = new[] {
						new {
							name = "TRADIE_ENV",
							Value = resourceConfig.Environment,
						},
						new {
							name = "BUILD_HASH",
							Value = build.ImageId.ImageTag
						}
					},
					logConfiguration = new {
						logDriver = "awslogs",
						options = new Dictionary<string, string>() {
							// Invalid identifiers used, so use a Dictionary.
							{"awslogs-group", logs.Name},
							{"awslogs-region", resourceConfig.Region},
							{"awslogs-stream-prefix", "tradie-scanner"},
						}
					},
				},
			}),
		});

		var service = new EcsService(stack, "scanner-service", new EcsServiceConfig() {
			Cluster = ecs.Cluster.Arn,
			Name = "scanner-service",
			DesiredCount = 1,
			LaunchType = "EC2",
			TaskDefinition = taskDef.Arn,
			EnableEcsManagedTags = true
			/*NetworkConfiguration = new EcsServiceNetworkConfiguration() {
				Subnets = new[] { network.PublicSubnets[0].Id },
				SecurityGroups = new[] { network.InternalTrafficOnlySecurityGroup.Id }
			},*/
		});
	}
}