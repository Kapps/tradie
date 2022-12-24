using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.IamRole;
using HashiCorp.Cdktf.Providers.Aws.LambdaFunction;
using HashiCorp.Cdktf.Providers.Aws.LambdaPermission;
using HashiCorp.Cdktf.Providers.Aws.S3Bucket;
using HashiCorp.Cdktf.Providers.Aws.S3BucketNotification;
using HashiCorp.Cdktf.Providers.Aws.SqsQueue;
using HashiCorp.Cdktf.Providers.Aws.SsmParameter;
using System.Collections.Generic;
using System.Text.Json;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Packaging;
using Tradie.Infrastructure.Resources;
using Tradie.Infrastructure.Scanner;

namespace Tradie.Infrastructure.Analyzer;

public class AnalyzerLambda {
	public readonly S3Bucket AnalyzedItemBucket;

	public AnalyzerLambda(
		TerraformStack stack,
		ResourceConfig resourceConfig,
		Permissions permissions,
		ScannerService scanner,
		Network network,
		ItemStream itemStream,
		PackagedBuild packagedBuild
	) {

		var build = packagedBuild.GetPublishedPackage()
			.GetAwaiter().GetResult();
		
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
				permissions.AllowLoggingPolicy,
				permissions.ReadConfigPolicy,
				permissions.CreateBasicLambdaPolicy(dlq.Arn),
				new IamRoleInlinePolicy() {
					Name = "analyzer-s3",
					Policy = JsonSerializer.Serialize(new {
						Version = Permissions.PolicyVersion,
						Statement = new[] {
							new {
								Effect = "Allow",
								Action = new[] {"s3:*"},
								Resource = new[] {
									this.AnalyzedItemBucket.Arn,
									$"{this.AnalyzedItemBucket.Arn}/*",
									scanner.ChangeBucket.Arn,
									$"{scanner.ChangeBucket.Arn}/*"
								}
							}
						}
					})
				},
				new IamRoleInlinePolicy() {
					Name = "analyzer-kinesis",
					Policy = JsonSerializer.Serialize(new {
						Version = Permissions.PolicyVersion,
						Statement = new[] {
							new {
								Effect = "Allow",
								Action = new[] {"kinesis:*"},
								Resource = new[] {
									itemStream.KinesisStream.Arn
								}
							}
						}
					})
				}
			},
			AssumeRolePolicy = Permissions.LambdaAssumeRolePolicy,
		});

		/*var logs = new CloudwatchLogGroup(stack, "analyzer-log-group", new CloudwatchLogGroupConfig() {
			Name = "analyzer-logs",
			RetentionInDays = 14,
		});*/

		var lambda = new LambdaFunction(stack, "analyzer-lambda", new LambdaFunctionConfig() {
			Architectures = new[] {"x86_64"},
			VpcConfig = new LambdaFunctionVpcConfig() {
				SubnetIds = new[] {network.PrivateSubnets[0].Id},
				SecurityGroupIds = new[] {network.InternalTrafficOnlySecurityGroup.Id},
			},
			FunctionName = "analyzer",
			Role = role.Arn,
			Environment = new LambdaFunctionEnvironment() {
				Variables = new Dictionary<string, string>() {
					{"TRADIE_ENV", resourceConfig.Environment},
					{"DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false"},
					{"BUILD_HASH", build.ImageId.ImageTag}
				},
			},
			DeadLetterConfig = new LambdaFunctionDeadLetterConfig() {
				TargetArn = dlq.Arn
			},
			ReservedConcurrentExecutions = 5,
			ImageUri = build.TaggedImageUri,
			MemorySize = 1536,
			PackageType = "Image",
			Timeout = 120,
			//DependsOn = new[] {repo.BuildResource},
		});

		var triggerPerm = new LambdaPermission(stack, "analyzer-s3-perm", new LambdaPermissionConfig() {
			Action = "lambda:InvokeFunction",
			StatementId = "AllowExecutionFromS3Bucket",
			FunctionName = lambda.Arn,
			Principal = "s3.amazonaws.com",
			SourceArn = scanner.ChangeBucket.Arn
		});

		var trigger = new S3BucketNotification(stack, "analyzer-s3-trigger", new S3BucketNotificationConfig() {
			Bucket = scanner.ChangeBucket.Bucket,
			DependsOn = new[] {triggerPerm},
			LambdaFunction = new IS3BucketNotificationLambdaFunction[] {
				new S3BucketNotificationLambdaFunction() {
					Events = new[] {
						"s3:ObjectCreated:*",
					},
					LambdaFunctionArn = lambda.Arn,
				}
			}
		});
	}
}