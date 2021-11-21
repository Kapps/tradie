using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Ec2;
using HashiCorp.Cdktf.Providers.Aws.Vpc;

namespace Tradie.Infrastructure.Resources {
	/// <summary>
	/// Handles private and public subnet routing through a NAT Instance and other gateways.
	/// </summary>
	public class Routing {
		public Routing(
			TerraformStack stack, 
			Network network, 
			Ecs ecs,
			IAmazonSimpleSystemsManagement ssmClient
		) {
			// Create a NAT instance instead of a NAT gateway, as NAT gateways are too expensive.
			// Can't use an ECS image for this because it doesn't work as a NAT instance?
			// So use a regular one and add ECS resources.
			var natAmi = ssmClient.GetParameterAsync(new GetParameterRequest() {
				Name = "/aws/service/ami-amazon-linux-latest/amzn2-ami-hvm-arm64-gp2",
			}).Result.Parameter.Value;

			var natSg = new SecurityGroup(stack, "nat-sg", new SecurityGroupConfig() {
				VpcId = network.Vpc.Id,
				Name = "nat-sg",
				Ingress = new[] {
					network.AllInternalTrafficIngress,
				},
				Egress = new[] {
					network.AllOutgoingTrafficEgress,
				},
			});
			
			var natInstance = new Instance(stack, "nat-instance", new InstanceConfig() {
				/*LaunchTemplate = new InstanceLaunchTemplate() {
					Id = ecs.LaunchTemplate.Id,
					Name = ecs.LaunchTemplate.Name,
					Version = "$Latest",
				},*/
				Ami = natAmi,
				IamInstanceProfile = ecs.EcsInstanceProfile.Name,
				InstanceType = "t4g.micro",
				VpcSecurityGroupIds = new[] { natSg.Id },
				AssociatePublicIpAddress = true,
				SourceDestCheck = false,
				SubnetId = network.PublicSubnets[0].Id,
				UserData =
					$"#!/bin/bash\n" +
					$"sudo amazon-linux-extras disable docker\n" +
					$"sudo amazon-linux-extras install -y ecs\n" +
					$"sudo systemctl enable --now --no-block ecs\n" +
					$"echo ECS_CLUSTER={ecs.Cluster.Name} >> /etc/ecs/ecs.config\n" +
					$"sysctl -w net.ipv4.ip_forward=1\n" +
					$"/sbin/iptables -t nat -A POSTROUTING -o eth0 -j MASQUERADE\n" +
					$"echo \"net.ipv4.ip_forward = 1\" >> /etc/sysctl.conf\n" +
					$"yum install iptables-services -y\n" +
					$"service iptables save\n" +
					$"service iptables start\n" +
					$"sudo chkconfig iptables on\n"
			});
			
			var egressGateway = new EgressOnlyInternetGateway(stack, "egress-gateway", new EgressOnlyInternetGatewayConfig() {
				VpcId = network.Vpc.Id,
			});

			var inetGateway = new InternetGateway(stack, "internet-gateway", new InternetGatewayConfig() {
				VpcId = network.Vpc.Id,
			});

			var natRouteTable = new RouteTable(stack, "nat-route-table", new RouteTableConfig() {
				VpcId = network.Vpc.Id,
				Route = new[] {
					new RouteTableRoute() {
						Ipv6CidrBlock = "",
						CidrBlock = "0.0.0.0/0",
						InstanceId = natInstance.Id,
						EgressOnlyGatewayId = "", GatewayId = "", CarrierGatewayId = "",
						LocalGatewayId = "", NatGatewayId = "", NetworkInterfaceId = "",
						TransitGatewayId = "", VpcEndpointId = "", VpcPeeringConnectionId = "",
						DestinationPrefixListId = ""
					},
					/*new RouteTableRoute() {
						Ipv6CidrBlock = "::/0",
						CidrBlock = "",
						EgressOnlyGatewayId = egressGateway.Id,
						GatewayId = "", InstanceId = "", CarrierGatewayId = "",
						LocalGatewayId = "", NatGatewayId = "", NetworkInterfaceId = "",
						TransitGatewayId = "", VpcEndpointId = "", VpcPeeringConnectionId = "",
						DestinationPrefixListId = ""
					},*/
				},
			});
			
			var igwRouteTable = new RouteTable(stack, "igw-route-table", new RouteTableConfig() {
				VpcId = network.Vpc.Id,
				Route = new[] {
					new RouteTableRoute() {
						CidrBlock = "0.0.0.0/0", //network.Vpc.CidrBlock,
						GatewayId = inetGateway.Id,
						EgressOnlyGatewayId = "", InstanceId = "", CarrierGatewayId = "",
						LocalGatewayId = "", NatGatewayId = "", NetworkInterfaceId = "",
						TransitGatewayId = "", VpcEndpointId = "", VpcPeeringConnectionId = "",
						DestinationPrefixListId = "", Ipv6CidrBlock = "",
					},
				},
			});

			for(int i = 0; i < network.PrivateSubnets.Length; i++) {
				new RouteTableAssociation(stack, $"priv-subnet-table-{i}", new RouteTableAssociationConfig() {
					RouteTableId = natRouteTable.Id,
					SubnetId = network.PrivateSubnets[i].Id,
				});
			}
			
			for(int i = 0; i < network.PublicSubnets.Length; i++) {
				new RouteTableAssociation(stack, $"pub-subnet-table-{i}", new RouteTableAssociationConfig() {
					RouteTableId = igwRouteTable.Id,
					SubnetId = network.PublicSubnets[i].Id,
				});
			}
		}
	}
}