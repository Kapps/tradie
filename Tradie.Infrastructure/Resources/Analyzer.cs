using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.CloudWatch;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using HashiCorp.Cdktf.Providers.Aws.Iam;
using HashiCorp.Cdktf.Providers.Aws.LambdaFunction;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Sqs;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using System.IO;
using System.Text.Json;

namespace Tradie.Infrastructure.Resources {
	public class Analyzer {
		public readonly S3Bucket AnalyzedItemBucket;
		public readonly EcrProjectRepository ProjectRepository;

		public Analyzer(
			TerraformStack stack,
			ResourceConfig resourceConfig,
			Permissions permissions,
			Scanner scanner,
			Network network
		) {
			this.AnalyzedItemBucket = new S3Bucket(stack, "analyzed-changesets-bucket", new S3BucketConfig() {
				Bucket = "analyzed-changesets",
				ForceDestroy = true,
				Versioning = new S3BucketVersioning() {
					Enabled = false,
				},
				LifecycleRule = new[] {
					new S3BucketLifecycleRule() {
						Enabled = true,
						Expiration = new S3BucketLifecycleRuleExpiration() {
							Days = 7,
						},
						AbortIncompleteMultipartUploadDays = 7,
						NoncurrentVersionExpiration = new S3BucketLifecycleRuleNoncurrentVersionExpiration() {
							Days = 7,
						},
					},
				},
			});

			var ssm = new SsmParameter(stack, "analyzed-changesets-ssm", new SsmParameterConfig() {
				Name = "Config.AnalyzedChangeSetBucket",
				Value = this.AnalyzedItemBucket.Bucket!,
				Type = "String",
			});
			
			var dlq = new SqsQueue(stack, "analyzer-dlq", new SqsQueueConfig() {
				Name = "analyzer-dlq",
			});
			
			var role = new IamRole(stack, "analyzer-task-role", new IamRoleConfig() {
				Name = "analyzer-task-role",
				InlinePolicy = new[] {
					permissions.InlineLogPolicy,
					permissions.ReadConfigPolicy,
					new IamRoleInlinePolicy() {
						Name = "analyzer-s3",
						Policy = JsonSerializer.Serialize(new {
							Version = Permissions.PolicyVersion,
							Statement = new[] {
								new {
									Effect = "Allow",
									Action = new[] { "s3:*" },
									Resource = new[] {
										this.AnalyzedItemBucket.Arn,
										$"{this.AnalyzedItemBucket.Arn}/*",
										scanner.ChangeBucket.Arn,
										$"{scanner.ChangeBucket.Arn}/*"
									}
								}, new {
									Effect = "Allow",
									Action = new[] {
										"sqs:SendMessage",
										"sqs:ReceiveMessage",
										"sqs:DeleteMessage",
										"sqs:ChangeMessageVisibility"
									},
									Resource = new[] {
										dlq.Arn
									}
								}, new {
									Effect = "Allow",
									Action = new[] {
										"ec2:CreateNetworkInterface",
										"ec2:DescribeNetworkInterfaces",
										"ec2:DeleteNetworkInterface"
									},
									Resource = new[] { "*" }
								}, new {
									Effect = "Allow",
									Action = new[] {
										"ecr:BatchGetImage",
										"ecr:GetDownloadUrlForLayer"
									},
									Resource = new[] { "*" }
								}
							},
						})
					}
				},
				AssumeRolePolicy = Permissions.LambdaAssumeRolePolicy,
			});

			var logs = new CloudwatchLogGroup(stack, "analyzer-log-group", new CloudwatchLogGroupConfig() {
				Name = "analyzer-logs",
				RetentionInDays = 14,
			});

			var repo = new EcrProjectRepository(stack, "analyzer", "Tradie.Analyzer", resourceConfig);

			var lambda = new LambdaFunction(stack, "analyzer-lambda", new LambdaFunctionConfig() {
				Architectures = new[] { "arm64" },
				VpcConfig = new LambdaFunctionVpcConfig() {
					SubnetIds = new[] { network.PublicSubnets[0].Id },
					SecurityGroupIds = new[] { network.InternalTrafficOnlySecurityGroup.Id },
				},
				FunctionName = "analyzer",
				Role = role.Arn,
				Environment = new LambdaFunctionEnvironment() {
					Variables = new[] {
						new {
							TRADIE_ENV=resourceConfig.Environment
						}
					}
				},
				DeadLetterConfig = new LambdaFunctionDeadLetterConfig() {
					TargetArn = dlq.Arn
				},
				Runtime = "provided.al2",
				ImageUri = $"{repo.EcrRepo.RepositoryUrl}:latest",
				MemorySize = 512,
				PackageType = "Image",
				Timeout = 300,
			});
		}
	}
}