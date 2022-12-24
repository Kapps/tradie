using HashiCorp.Cdktf;
using System.Collections.Generic;
using System.Text.Json;

namespace Tradie.Infrastructure.Resources {
	public class ItemLogBuilder {
		public ItemLogBuilder(
			TerraformStack stack,
			ResourceConfig resourceConfig
			//Permissions permissions,
			//ItemStream itemStream,
			//Network network
		) {
			/*var dlq = new SqsQueue(stack, "logbuilder-dlq", new SqsQueueConfig() {
				Name = "logbuilder-dlq",
			});
			
			var role = new IamRole(stack, "logbuilder-task-role", new IamRoleConfig() {
				Name = "logbuilder-task-role",
				InlinePolicy = new[] {
					permissions.AllowLoggingPolicy,
					permissions.ReadConfigPolicy,
					permissions.WriteConfigPolicy,
					permissions.CreateBasicLambdaPolicy(dlq.Arn),
					new IamRoleInlinePolicy() {
						Name = "logbuilder-kinesis",
						Policy = JsonSerializer.Serialize(new {
							Version = Permissions.PolicyVersion,
							Statement = new[] {
								new {
									Effect = "Allow",
									Action = new[] { "kinesis:*" },
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
			

			var lambda = new LambdaFunction(stack, "logbuilder-lambda", new LambdaFunctionConfig() {
				Architectures = new[] { "arm64" },
				VpcConfig = new LambdaFunctionVpcConfig() {
					SubnetIds = new[] { network.PrivateSubnets[0].Id },
					SecurityGroupIds = new[] { network.InternalTrafficOnlySecurityGroup.Id },
				},
				FunctionName = "logbuilder",
				Role = role.Arn,
				Environment = new LambdaFunctionEnvironment() {
					Variables = new Dictionary<string, string>() {
						{ "TRADIE_ENV", resourceConfig.Environment },
						{ "DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false" },
						{ "BUILD_HASH", repo.HashTag }
					},
				},
				DeadLetterConfig = new LambdaFunctionDeadLetterConfig() {
					TargetArn = dlq.Arn
				},
				ReservedConcurrentExecutions = 1,
				ImageUri = repo.EcrImageUri,
				Tags = new[] { repo.BuildResource.Id },
				MemorySize = 1024,
				PackageType = "Image",
				Timeout = 240,
				DependsOn = new[] { repo.BuildResource }
			});

			var scheduleRule = new CloudwatchEventRule(stack, "logbuilder-schedule-rule", new CloudwatchEventRuleConfig() {
				Name = "logbuilder-schedule-rule",
				Description = "Rule to trigger the LogBuilder to update the snapshot.",
				IsEnabled = true,
				ScheduleExpression = "rate(5 minutes)"
			});

			var scheduleTarget = new CloudwatchEventTarget(stack, "logbuilder-schedule-target", new CloudwatchEventTargetConfig() {
				Arn = lambda.Arn,
				Rule = scheduleRule.Id
			});
			
			var triggerPerm = new LambdaPermission(stack, "logbuilder-cron-perm", new LambdaPermissionConfig() {
				Action = "lambda:InvokeFunction",
				StatementId = "AllowExecutionFromCloudWatch",
				FunctionName = lambda.Arn,
				Principal = "events.amazonaws.com",
				SourceArn = scheduleRule.Arn
			});*/
		}
	}
}