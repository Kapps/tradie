using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using System;

namespace Tradie.Infrastructure.Resources {
	/// <summary>
	/// Network configuration common to many resources, such as VPC and Subnet data.
	/// </summary>
	public class Network {
		/// <summary>
		/// The VPC that all resources should be using.
		/// </summary>
		public readonly Vpc Vpc;
		/// <summary>
		/// The public subnets inside the VPC.
		/// </summary>
		public readonly Subnet[] PublicSubnets;
		/// <summary>
		/// The private subnets inside the VPC.
		/// </summary>
		public readonly Subnet[] PrivateSubnets;
		/// <summary>
		/// A SecurityGroupEgress to allow all outgoing traffic from the resource.
		/// </summary>
		public readonly SecurityGroupEgress AllOutgoingTrafficEgress;
		
		public Network(TerraformStack stack, ResourceConfig config) {
			this.Vpc = new Vpc(stack, "vpc", new VpcConfig() {
				CidrBlock = "10.200.0.0/16",
			});

			this.PrivateSubnets = new[] {
				new Subnet(stack, "priv-subnet-1", new SubnetConfig() {
					CidrBlock = "10.200.1.0/24",
					VpcId = this.Vpc.Id,
					AvailabilityZone = $"{config.Region}a",
				}),
				new Subnet(stack, "priv-subnet-2", new SubnetConfig() {
					CidrBlock = "10.200.2.0/24",
					VpcId = this.Vpc.Id,
					AvailabilityZone = $"{config.Region}b",
				}),
			};

			this.PublicSubnets = new[] {
				new Subnet(stack, "pub-subnet-1", new SubnetConfig() {
					CidrBlock = "10.200.101.0/24",
					VpcId = this.Vpc.Id,
					AvailabilityZone = $"{config.Region}a",
				}),
				new Subnet(stack, "pub-subnet-2", new SubnetConfig() {
					CidrBlock = "10.200.102.0/24",
					VpcId = this.Vpc.Id,
					AvailabilityZone = $"{config.Region}b",
				}),
			};

			this.AllOutgoingTrafficEgress = new SecurityGroupEgress() {
				FromPort = 0, ToPort = 0,
				Protocol = "-1", CidrBlocks = new[] { "0.0.0.0/0" },
				Description = "Allow all egress traffic.",
				Ipv6CidrBlocks = Array.Empty<string>(),
				PrefixListIds = Array.Empty<string>(),
				SecurityGroups = Array.Empty<string>(),
				SelfAttribute = false,
			};
		}
	}
}