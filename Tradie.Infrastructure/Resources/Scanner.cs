﻿using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.CloudWatch;
using HashiCorp.Cdktf.Providers.Aws.Ecr;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Iam;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using HashiCorp.Cdktf.Providers.Null;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Tradie.Infrastructure.Resources {
    public class Scanner {
		public readonly S3Bucket ChangeBucket;
		public readonly EcrProjectRepository Repo;
		
        public Scanner(
	        TerraformStack stack,
	        Network network,
	        Ecs ecs,
	        ResourceConfig resourceConfig,
	        Permissions permissions
	    ) {
	        this.ChangeBucket = new S3Bucket(stack, "raw-changesets-bucket", new S3BucketConfig() {
				Bucket = "raw-changesets",
				ForceDestroy = true,
				Versioning = new S3BucketVersioning() {
					Enabled = false,
				}
			});	

			new SsmParameter(stack, "raw-changesets-ssm", new SsmParameterConfig() {
				Name = "Config.ChangeSetBucket",
				Value = this.ChangeBucket.Bucket,
				Type = "String",
			});

			this.Repo = new EcrProjectRepository(stack, "scanner", "Tradie.Scanner", resourceConfig);

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
									Action = new[] { "s3:*" },
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
									Action = new[] { "ssm:PutParameter" },
									Resource = new[] { "*" },
								}
							}
						}),
					}
				},
				AssumeRolePolicy = Permissions.EcsAssumeRolePolicy,
			});
			
			var taskDef = new EcsTaskDefinition(stack, "scanner-taskdef", new EcsTaskDefinitionConfig() {
				Cpu = "256",
				Memory = "512",
				NetworkMode = "host",
				Family = "scanner",
				TaskRoleArn = taskRole.Arn,
				RequiresCompatibilities = new [] { "EC2" },
				ExecutionRoleArn = permissions.ExecutionRole.Arn,
				DependsOn = new[] {this.Repo.BuildResource},
				ContainerDefinitions = JsonSerializer.Serialize(new[] {
					new {
						name = "tradie-scanner",
						this.Repo.Tag,
						image = this.Repo.Tag,
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
}
