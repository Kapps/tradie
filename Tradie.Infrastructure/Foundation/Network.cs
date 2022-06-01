using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Ecs;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using System;

namespace Tradie.Infrastructure.Foundation;

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

	/// <summary>
	/// A SecurityGroupIngress that allows all incoming traffic from the internal network.
	/// </summary>
	public readonly SecurityGroupIngress AllInternalTrafficIngress;

	/// <summary>
	/// A security group that allows all internal traffic to hit this instance, and no external traffic.
	/// All egress traffic is allowed.
	/// </summary>
	public readonly SecurityGroup InternalTrafficOnlySecurityGroup;

	public Network(TerraformStack stack, ResourceConfig config) {
		this.Vpc = new Vpc(stack, "vpc", new VpcConfig() {
			CidrBlock = "10.200.0.0/16",
			AssignGeneratedIpv6CidrBlock = true,
			EnableDnsHostnames = true,
			EnableDnsSupport = true,
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
			Protocol = "-1", CidrBlocks = new[] {"0.0.0.0/0"},
			Description = "Allow all egress traffic.",
			Ipv6CidrBlocks = Array.Empty<string>(),
			PrefixListIds = Array.Empty<string>(),
			SecurityGroups = Array.Empty<string>(),
			SelfAttribute = false,
		};

		this.AllInternalTrafficIngress = new SecurityGroupIngress() {
			CidrBlocks = new[] {this.Vpc.CidrBlock},
			FromPort = 0, ToPort = 0,
			Protocol = "-1",
			Description = "Allow all internal traffic ingress.",
			Ipv6CidrBlocks = new[] {this.Vpc.Ipv6CidrBlock},
			SecurityGroups = Array.Empty<string>(),
			SelfAttribute = false,
			PrefixListIds = Array.Empty<string>()
		};

		this.InternalTrafficOnlySecurityGroup = new SecurityGroup(stack, "internal-only-sg",
			new SecurityGroupConfig() {
				Name = "internal-only-sg",
				Egress = new[] {this.AllOutgoingTrafficEgress},
				Ingress = new[] {this.AllInternalTrafficIngress},
				VpcId = this.Vpc.Id,
			});
	}
}