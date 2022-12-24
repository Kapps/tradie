using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.AlbListenerRule;
using HashiCorp.Cdktf.Providers.Aws.AlbTargetGroup;
using HashiCorp.Cdktf.Providers.Aws.AlbTargetGroupAttachment;
using HashiCorp.Cdktf.Providers.Aws.IamRole;
using HashiCorp.Cdktf.Providers.Aws.LambdaFunction;
using HashiCorp.Cdktf.Providers.Aws.LambdaPermission;
using HashiCorp.Cdktf.Providers.Aws.SqsQueue;
using System.Collections.Generic;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Packaging;
using Tradie.Infrastructure.Resources;

namespace Tradie.Infrastructure.Web;

public class WebLambda {
	public WebLambda(
		TerraformStack stack,
		ResourceConfig resourceConfig,
		FoundationStack foundation,
		PackagedBuild packagedBuild
	) {

		var alb = foundation.Alb;
		var network = foundation.Network;
		var permissions = foundation.Permissions;
		
		var build = packagedBuild.GetPublishedPackage()
			.GetAwaiter().GetResult();
		
		var dlq = new SqsQueue(stack, "web-dlq", new SqsQueueConfig() {
			Name = "web-dlq",
		});

		var role = new IamRole(stack, "web-task-role", new IamRoleConfig() {
			Name = "web-task-role",
			InlinePolicy = new[] {
				permissions.AllowLoggingPolicy,
				permissions.ReadConfigPolicy,
				permissions.CreateBasicLambdaPolicy(dlq.Arn)
			},
			AssumeRolePolicy = Permissions.LambdaAssumeRolePolicy,
		});

		/*var logs = new CloudwatchLogGroup(stack, "analyzer-log-group", new CloudwatchLogGroupConfig() {
			Name = "analyzer-logs",
			RetentionInDays = 14,
		});*/

		var lambda = new LambdaFunction(stack, "web-lambda", new LambdaFunctionConfig() {
			Architectures = new[] {"x86_64"},
			VpcConfig = new LambdaFunctionVpcConfig() {
				SubnetIds = new[] {network.PrivateSubnets[0].Id},
				SecurityGroupIds = new[] {network.InternalTrafficOnlySecurityGroup.Id},
			},
			FunctionName = "web",
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
			Timeout = 60,
			//DependsOn = new[] {repo.BuildResource},
		});

		var httpTg = new AlbTargetGroup(stack, "http-tg", new AlbTargetGroupConfig() {
			Name = "http-tg",
			//Protocol = "HTTP",
			VpcId = network.Vpc.Id,
			//Port = 80,
			TargetType = "lambda",
			LoadBalancingAlgorithmType = "least_outstanding_requests",
		});

		var triggerPerm = new LambdaPermission(stack, "analyzer-s3-perm", new LambdaPermissionConfig() {
			Action = "lambda:InvokeFunction",
			StatementId = "AllowExecutionFromALB",
			FunctionName = lambda.FunctionName,
			Principal = "elasticloadbalancing.amazonaws.com",
			SourceArn = httpTg.Arn
		});
		
		var httpTga = new AlbTargetGroupAttachment(stack, "http-tga", new AlbTargetGroupAttachmentConfig() {
			TargetGroupArn = httpTg.Arn,
			TargetId = lambda.Arn,
			DependsOn = new[] { triggerPerm }
		});
		
		var listenerRule = new AlbListenerRule(stack, "lambda-rule", new AlbListenerRuleConfig() {
			ListenerArn = alb.HttpsListener.Arn,
			Priority = 10,
			Condition = new[] { 
				new AlbListenerRuleCondition() {
					PathPattern	= new AlbListenerRuleConditionPathPattern() {
						Values = new[] { "*" } // All traffic.
					}
				}
			},
			Action = new[] {
				new AlbListenerRuleAction() {
					Type = "forward",
					TargetGroupArn = httpTg.Arn
					/*Forward = new AlbListenerRuleActionForward() {
						TargetGroup = new[] { httpTg.Arn }
					}*/
				}
			}
		});

		/*var trigger = new S3BucketNotification(stack, "analyzer-s3-trigger", new S3BucketNotificationConfig() {
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
		});*/
	}
}