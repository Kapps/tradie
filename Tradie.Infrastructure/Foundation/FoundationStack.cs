using Amazon.JSII.Runtime.Deputy;
using Amazon.SimpleSystemsManagement;
using Constructs;
using HashiCorp.Cdktf;
using Tradie.Infrastructure.Resources;

namespace Tradie.Infrastructure.Foundation;

public class FoundationStack : StackBase {
	public Permissions Permissions { get; set; }
	public Network Network { get; set; }
	public DbParams Database { get; set; }
	public Cache Cache { get; set; }
	public Ecs EcsCluster { get; set; }
	public Routing Routing { get; set; }
	public Storage Storage { get; set; }
	public Alb Alb { get; set; }


	public FoundationStack(Construct scope, string id, ResourceConfig resourceConfig) : base(scope, id, resourceConfig) {
		var ssm = new AmazonSimpleSystemsManagementClient();

		this.Permissions = new(this);
		this.Network = new(this, resourceConfig);
		this.Database = new(this);
		this.Cache = new(this);
		this.Storage = new(this);

		this.EcsCluster = new(this, this.Network, ssm);
		this.Routing = new(this, this.Network, this.EcsCluster, ssm);
		this.Alb = new(this, this.Network, resourceConfig);
	}
}