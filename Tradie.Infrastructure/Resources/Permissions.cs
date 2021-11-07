using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Iam;
using System;
using System.Text.Json;

namespace Tradie.Infrastructure.Resources {
	public class Permissions {
		public const string PolicyVersion = "2012-10-17";
		
		/// <summary>
		/// An IAM role for an arbitrary task that needs to pull a Docker image and run.
		/// </summary>
		public readonly IamRole ExecutionRole;

		/// <summary>
		/// Inline policy to allow creating log streams and putting events.
		/// </summary>
		public readonly IIamRoleInlinePolicy InlineLogPolicy = new IamRoleInlinePolicy() {
			Name = "allow-logs",
			Policy = JsonSerializer.Serialize(new {
				Version = PolicyVersion,
				Statement = new[] {
					new {
						Effect = "Allow",
						Action = new[] { "logs:CreateLogStream", "logs:PutLogEvents" },
						Resource = new[] { "*" },
					}
				}
			})
		};

		/// <summary>
		/// Inline policy to allow retrieval of SSM parameters for config purposes.
		/// </summary>
		public readonly IIamRoleInlinePolicy ReadConfigPolicy = new IamRoleInlinePolicy() {
			Name = "read-ssm",
			Policy = JsonSerializer.Serialize(new {
				Version = PolicyVersion,
				Statement = new[] {
					new {
						Effect = "Allow",
						Action = new[] {
							"ssm:GetParameter",
							"ssm:GetParameters",
							"ssm:GetParameterHistory",
							"ssm:GetParametersByPath"
						},
						Resource = new[] { "*" },
					}
				}
			})
		};

		/// <summary>
		/// Policy string to allow assuming an ECS Task role.
		/// </summary>
		public static readonly string AssumeRolePolicy = JsonSerializer.Serialize(new {
			Version = PolicyVersion,
			Statement = new[] {
				new {
					Action = "sts:AssumeRole",
					Effect = "Allow",
					Sid = "",
					Principal = new {
						Service = "ecs-tasks.amazonaws.com",
					}
				}
			}
		});

		public Permissions(TerraformStack stack) {
			this.ExecutionRole = new IamRole(stack, "scanner-execution-role", new IamRoleConfig() {
				Name = "execution-role",
				InlinePolicy = new IIamRoleInlinePolicy[] {
					new IamRoleInlinePolicy() {
						Name = "allow-ecr-pull",
						Policy = JsonSerializer.Serialize(new {
							Version = PolicyVersion,
							Statement = new[] {
								new {
									Effect = "Allow",
									Action = new[] {
										"ecr:GetAuthorizationToken",
										"ecr:BatchCheckLayerAvailability",
										"ecr:GetDownloadUrlForLayer",
										"ecr:BatchGetImage",
										"logs:CreateLogStream",
										"logs:PutLogEvents",
									}
								}
							},
							Resource = new[] { "*" },
						}),
					}	
				},
				AssumeRolePolicy = AssumeRolePolicy,
			});
		}
	}
}