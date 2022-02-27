using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Rds;
using HashiCorp.Cdktf.Providers.Aws.Ssm;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using HashiCorp.Cdktf.Providers.Random;
using System;
using System.Linq;

namespace Tradie.Infrastructure.Resources {
	public class Rds {
		public Rds(TerraformStack stack, Network network, ResourceConfig config) {

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
					new SecurityGroupIngress() {
						CidrBlocks = new[] { $"{config.LocalIpAddress}/32" },
						FromPort = 5432, ToPort = 5432,
						Protocol = "TCP",
						Description = "Allow Postgres traffic from the dev host.",
						Ipv6CidrBlocks = Array.Empty<string>(),
						SecurityGroups = Array.Empty<string>(),
						SelfAttribute = false,
						PrefixListIds = Array.Empty<string>()
					}
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

			var rds = new DbInstance(stack, "rds", new DbInstanceConfig() {
				Identifier = "core",
				DbSubnetGroupName = subnetGroup.Name,
				Engine = "postgres",
				Name = "tradie",
				Username = "tradie",
				Password = passwordResource.Result,
				MultiAz = false,
				EngineVersion = "14.1",
				InstanceClass = "t4g.small",
				VpcSecurityGroupIds = new[] { securityGroup.Id },
				ApplyImmediately = true,
				AllocatedStorage = 50,
				PubliclyAccessible = true,
				SkipFinalSnapshot = true // TODO: Remove this if ever getting to for realsies.
			});
			
			/*var cluster = new RdsCluster(stack, "rds-cluster", new RdsClusterConfig() {
				Engine = "aurora-postgresql",
				ApplyImmediately = true,
				ClusterIdentifier = "core-cluster",
				DatabaseName = "tradie",
				EngineMode = "serverless",
				DbSubnetGroupName = subnetGroup.Name,
				MasterPassword = passwordResource.Result,
				MasterUsername = "tradie",
				VpcSecurityGroupIds = new[] { securityGroup.Id },
				ScalingConfiguration = new RdsClusterScalingConfiguration() {
					MinCapacity = 2,
					MaxCapacity = 4,
					AutoPause = true,
					SecondsUntilAutoPause = 300,
					TimeoutAction = "ForceApplyCapacityChange",
				},
				EnableHttpEndpoint = true,
				SkipFinalSnapshot = true,
			});*/
			
			var ssmHost = new SsmParameter(stack, "ssm-dbhost", new SsmParameterConfig() {
				Name = "Config.DbHost",
				Type = "String",
				// Value = cluster.Endpoint,
				Value = rds.Endpoint,
			});
			
			var ssmUser = new SsmParameter(stack, "ssm-dbuser", new SsmParameterConfig() {
				Name = "Config.DbUser",
				Type = "String",
				// Value = cluster.MasterUsername!,
				Value = rds.Username!,
			});

			var ssmPass = new SsmParameter(stack, "ssm-dbpass", new SsmParameterConfig() {
				Name = "Config.DbPass",
				Type = "String",
				// Value = cluster.MasterPassword!,
				Value = rds.Password!,
			});
			
			var ssmName = new SsmParameter(stack, "ssm-dbname", new SsmParameterConfig() {
				Name = "Config.DbName",
				Type = "String",
				// Value = cluster.DatabaseName!
				Value = rds.Name
			});
		}
	}
}