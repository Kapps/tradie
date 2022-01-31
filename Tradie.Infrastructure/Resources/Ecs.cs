using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Autoscaling;
using HashiCorp.Cdktf.Providers.Aws.Ec2;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Iam;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using System.Text;

namespace Tradie.Infrastructure.Resources {
	public class Ecs {
		/// <summary>
		/// The launch template to use for EC2 instances inside the primary ECS cluster.
		/// </summary>
		public readonly LaunchTemplate LaunchTemplate;
		/// <summary>
		/// The primary ECS cluster.
		/// </summary>
		public readonly EcsCluster Cluster;
		/// <summary>
		/// The instance profile to use for instances within the primary ECS cluster.
		/// </summary>
		public readonly IamInstanceProfile EcsInstanceProfile;
		/// <summary>
		/// The autoscaling group that launches instances for the primary cluster.
		/// </summary>
		public readonly AutoscalingGroup ClusterAsg;
		
		public Ecs(TerraformStack stack, Network network, IAmazonSimpleSystemsManagement ssmClient) {
			var instanceAmi = ssmClient.GetParameterAsync(new GetParameterRequest() {
				Name = "/aws/service/ecs/optimized-ami/amazon-linux-2/arm64/recommended/image_id",
			}).Result.Parameter.Value;
				
			
			this.Cluster = new EcsCluster(stack, "ecs", new EcsClusterConfig() {
				Name  = $"primary-cluster",
				//CapacityProviders = new string[] { "FARGATE" },
			});

			var agentPolicy = new DataAwsIamPolicyDocument(stack, "ecs-agent-policy",
				new DataAwsIamPolicyDocumentConfig() {
					Statement = new[] {
						new DataAwsIamPolicyDocumentStatement() {
							Actions = new[] {"sts:AssumeRole"},
							Principals = new[] {
								new DataAwsIamPolicyDocumentStatementPrincipals() {
									Type = "Service",
									Identifiers = new[] { "ec2.amazonaws.com", "ecs.amazonaws.com" }
								}
							}
						}
					}
				});

			var agentRole = new IamRole(stack, "ecs-agent-role", new IamRoleConfig() {
				Name = "ecs-agent-role",
				AssumeRolePolicy = agentPolicy.Json,
			});

			var policyAttachment = new IamRolePolicyAttachment(stack, "ecs-role-policy-attach",
				new IamRolePolicyAttachmentConfig() {
					Role = agentRole.Name,
					PolicyArn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role",
				});

			this.EcsInstanceProfile = new IamInstanceProfile(stack, "ecs-instance-profile", new IamInstanceProfileConfig() {
				Name = "ecs-agent-profile",
				Role = agentRole.Name,
			});

			var instanceSg = new SecurityGroup(stack, "ecs-sg", new SecurityGroupConfig() {
				VpcId = network.Vpc.Id,
				Name = "ecs-sg",
				Ingress = new[] {
					network.AllInternalTrafficIngress,
				},
				Egress = new[] {
					network.AllOutgoingTrafficEgress,
				},
			});

			this.LaunchTemplate = new LaunchTemplate(stack, "launch-template", new LaunchTemplateConfig() {
				ImageId = instanceAmi,
				InstanceType = "t4g.micro",
				VpcSecurityGroupIds = new[] { instanceSg.Id },
				IamInstanceProfile = new LaunchTemplateIamInstanceProfile() {
					Arn = this.EcsInstanceProfile.Arn,
				},
				UserData = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(
					$"#!/bin/bash\n" +
					$"echo ECS_CLUSTER={this.Cluster.Name} >> /etc/ecs/ecs.config\n"
				)),
			});

			/*this.ClusterAsg = new AutoscalingGroup(stack, "ecs-asg", new AutoscalingGroupConfig() {
				Name = "ecs-asg",
				MinSize = 1,
				MaxSize = 1,
				DesiredCapacity = 1,
				VpcZoneIdentifier = network.PublicSubnets.Select(c=>c.Id).ToArray(),
				
				LaunchTemplate = new AutoscalingGroupLaunchTemplate() {
					Id = this.LaunchTemplate.Id,
					Version = "$Latest",
				},
			});*/
		}
	}
}