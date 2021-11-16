using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Rds;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using HashiCorp.Cdktf.Providers.Random;
using System;
using System.Linq;

namespace Tradie.Infrastructure.Resources {
	public class Rds {
		public Rds(TerraformStack stack, Network network) {

			var subnetGroup = new DbSubnetGroup(stack, "rds-subnet-group", new DbSubnetGroupConfig() {
				Name = "rds-subnet-group",
				SubnetIds = network.PrivateSubnets.Select(c=>c.Id).ToArray(),
			});

			var securityGroup = new SecurityGroup(stack, "rds-security-group", new SecurityGroupConfig() {
				VpcId = network.Vpc.Id,
				Name = "rds-security-group",
				Ingress = new [] {
					new SecurityGroupIngress() {
						CidrBlocks = new[] { network.Vpc.CidrBlockInput },
						FromPort = 5432, ToPort = 5432,
						Protocol = "TCP",
						Description = "Allow Postgres traffic from inside the VPC",
						Ipv6CidrBlocks = Array.Empty<string>(),
						SecurityGroups = Array.Empty<string>(),
						SelfAttribute = false,
						PrefixListIds = Array.Empty<string>()
					},
				},
				Egress = new[] {
					network.AllOutgoingTrafficEgress,
				}
			});

			var passwordResource = new Password(stack, "rds-password", new PasswordConfig() {
				Length = 32,
				Lower = true,
				Number = true,
				Special = false,
				Upper = true,
			});
			
			/*var db = new RdsClusterInstance(stack, "rds", new RdsClusterInstanceConfig() {
				Identifier = "core",
				DbSubnetGroupName = subnetGroup.Name,
				Engine = "aurora-postgresql",
				InstanceClass = "t4g.medium",
				EngineVersion = "10.14", // 13.4 on us-east-1
				MultiAz = false,
				Username = "tradie",
				Password = passwordResource.Result,
			});*/

			var cluster = new RdsCluster(stack, "rds-cluster", new RdsClusterConfig() {
				Engine = "aurora-postgresql",
				ApplyImmediately = true,
				ClusterIdentifier = "core-cluster",
				DatabaseName = "tradie",
				EngineMode = "serverless",
				DbSubnetGroupName = subnetGroup.Name,
				MasterPassword = passwordResource.Result,
				MasterUsername = "tradie",
				VpcSecurityGroupIds = new[] { securityGroup.Id },
				/*ClusterMembers = new [] {
					db.Arn,
				},*/
				ScalingConfiguration = new RdsClusterScalingConfiguration() {
					MinCapacity = 2,
					MaxCapacity = 4,
					AutoPause = true,
					SecondsUntilAutoPause = 300,
					TimeoutAction = "ForceApplyCapacityChange",
				}
			});

			var ssmHost = new SsmParameter(stack, "ssm-dbhost", new SsmParameterConfig() {
				Name = "Config.DbHost",
				Type = "String",
				Value = cluster.Endpoint,
			});
			
			var ssmUser = new SsmParameter(stack, "ssm-dbuser", new SsmParameterConfig() {
				Name = "Config.DbUser",
				Type = "String",
				Value = cluster.MasterUsername!,
			});

			var ssmPass = new SsmParameter(stack, "ssm-dbpass", new SsmParameterConfig() {
				Name = "Config.DbPass",
				Type = "String",
				Value = cluster.MasterPassword!,
			});
		}
	}
}