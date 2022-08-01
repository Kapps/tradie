using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using DeepEqual.Syntax;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Common.Services;
using Tradie.TestUtils;

namespace Tradie.Common.Tests.Services;

[TestClass]
public class ServiceRegistryTests : TestBase {
	protected override void Initialize() {
		var logger = TestUtils.TestUtils.CreateLogger<CloudMapServiceRegistry>();
		this._serviceRegistry = new(
			this._serviceDiscoveryClient.Object,
			logger,
			Mock.Of<IHostEnvironment>(c=>c.EnvironmentName == "Development")
		);
	}

	[TestMethod]
	public async Task TestRegister() {
		string endpoint = "https://dev.tradie.io:5000";
		var ct = CancellationToken.None;
		this._serviceDiscoveryClient.Setup(c => c.RegisterInstanceAsync(new RegisterInstanceRequest() {
			Attributes = new Dictionary<string, string>() {
				{"ENDPOINT", endpoint},
				{"TRADIE_ENVIRONMENT", "Development"},
				{"AWS_INSTANCE_PORT", "5000"},
				{"AWS_INSTANCE_CNAME", "dev.tradie.io"},
				{"SOME_ATTRIBUTE", "foo"}
			},
			ServiceId = "some service",
			InstanceId = "some-instance",
		}.DeepMatcher("CreatorRequestId"), ct)).ReturnsAsync(new RegisterInstanceResponse() {
			OperationId = "operation"
		});
		this._serviceDiscoveryClient.Setup(c => c.GetOperationAsync(new GetOperationRequest() {
			OperationId = "operation"
		}.DeepMatcher(), ct)).ReturnsAsync(new GetOperationResponse() {
			Operation = new Operation() {
				Status = OperationStatus.SUCCESS
			}
		});
		
		await this._serviceRegistry.RegisterService(
			"some service", 
			new ServiceProperties(endpoint, "some-instance", new() { { "SOME_ATTRIBUTE", "foo" }}),
			ct
		);
	}
	
	[TestMethod]
	public async Task TestDiscoverService() {
		string endpoint = "https://dev.tradie.io:5000";
		var ct = CancellationToken.None;
		this._serviceDiscoveryClient.Setup(c => c.DiscoverInstancesAsync(new DiscoverInstancesRequest() {
			QueryParameters = new Dictionary<string, string>() {
				{"SOME_ATTRIBUTE", "foo"},
				{"TRADIE_ENVIRONMENT", "Development"},
			},
			ServiceName = "some-service",
			HealthStatus = HealthStatusFilter.ALL,
			NamespaceName = TradieConfig.DiscoveryNamespace,
		}.DeepMatcher(), ct)).ReturnsAsync(new DiscoverInstancesResponse() {
			Instances = new() {
				new HttpInstanceSummary() {
					Attributes = new() {
						{ "ENDPOINT", endpoint }
					},
					InstanceId = "some-instance",
					HealthStatus = HealthStatus.HEALTHY
				}
			}
		});
		
		var resp = await this._serviceRegistry.DiscoverService(
			"some-service", 
			new() { { "SOME_ATTRIBUTE", "foo" }},
			ct
		);
		
		resp.ShouldDeepEqual(new AvailableService(endpoint, "some-instance", ServiceHealth.Healthy));
	}
	
	[TestMethod]
	public async Task TestDeregister() {
		var ct = CancellationToken.None;
		this._serviceDiscoveryClient.Setup(c => c.DeregisterInstanceAsync(new DeregisterInstanceRequest() {
			ServiceId = "some service",
			InstanceId = "some-instance",
		}.DeepMatcher(), ct)).ReturnsAsync(new DeregisterInstanceResponse() {
			OperationId = "operation"
		});
		this._serviceDiscoveryClient.Setup(c => c.GetOperationAsync(new GetOperationRequest() {
			OperationId = "operation"
		}.DeepMatcher(), ct)).ReturnsAsync(new GetOperationResponse() {
			Operation = new Operation() {
				Status = OperationStatus.SUCCESS
			}
		});
		
		await this._serviceRegistry.DeregisterService(
			"some service", 
			"some-instance",
			ct
		);
	}

	private CloudMapServiceRegistry _serviceRegistry = null!;
	private Mock<IAmazonServiceDiscovery> _serviceDiscoveryClient = null!;
}