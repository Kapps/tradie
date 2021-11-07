using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Vpc;

namespace Tradie.Infrastructure.Resources {
	/// <summary>
	/// Network configuration common to many resources, such as VPC and Subnet data.
	/// </summary>
	public class Network {
		public readonly Vpc Vpc;
		public readonly Subnet[] PublicSubnets;
		public readonly Subnet[] PrivateSubnets;
		
		public Network(TerraformStack stack) {
			this.Vpc = new Vpc(stack, "vpc", new VpcConfig() {
				CidrBlock = "10.200.0.0/16",
			});

			this.PrivateSubnets = new[] {
				new Subnet(stack, "priv-subnet-1", new SubnetConfig() {
					CidrBlock = "10.200.1.0/24",
					VpcId = this.Vpc.Id,
				}),
			};

			this.PublicSubnets = new[] {
				new Subnet(stack, "pub-subnet-1", new SubnetConfig() {
					CidrBlock = "10.200.101.0/24",
					VpcId = this.Vpc.Id,
				}),
			};
		}
	}
}