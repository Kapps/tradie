using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Tradie.Common.Services;

/// <summary>
/// A service registry to allow discovering instances of individual services.
/// </summary>
public interface IServiceRegistry {
	/// <summary>
	/// Discovers an instance of this service with the given attributes.
	/// </summary>
	Task<AvailableService> DiscoverService(string service, Dictionary<string, string> requiredAttributes, CancellationToken cancellationToken);
	/// <summary>
	/// Registers a new instance of this service with the given attributes.
	/// </summary>
	Task RegisterService(string service, ServiceProperties properties, CancellationToken cancellationToken);
	/// <summary>
	/// Deregisters a previously registered instance of this service.
	/// </summary>
	Task DeregisterService(string service, string instanceId, CancellationToken cancellationToken);
}

public class CloudMapServiceRegistry : IServiceRegistry {
	public CloudMapServiceRegistry(
		IAmazonServiceDiscovery serviceDiscoveryClient, 
		ILogger<CloudMapServiceRegistry> logger,
		IHostEnvironment environment
	) {
		this._serviceDiscoveryClient = serviceDiscoveryClient;
		this._logger = logger;
		this._environment = environment;
	}
	
	public async Task<AvailableService> DiscoverService(string service, Dictionary<string, string> requiredAttributes, CancellationToken cancellationToken) {
		var req = new DiscoverInstancesRequest() {
			NamespaceName = TradieConfig.DiscoveryNamespaceName,
			ServiceName = service,
			QueryParameters = new(
				new KeyValuePair<string, string>[] {
					new("TRADIE_ENVIRONMENT", this._environment.EnvironmentName),
				}.Concat(requiredAttributes)
			),
			HealthStatus = HealthStatusFilter.ALL
		};

		var resp = await this._serviceDiscoveryClient.DiscoverInstancesAsync(req, cancellationToken);
		if (resp.Instances.Count == 0) {
			throw new ServiceNotFoundException($"No available instances found for service {service}");
		}
		
		var instance = resp.Instances.First();
		var result = new AvailableService(
			instance.Attributes["ENDPOINT"],
			instance.InstanceId,
			FromHealthStatus(instance.HealthStatus)
		);
		return result;
	}

	private ServiceHealth FromHealthStatus(HealthStatus status) {
		if(status == HealthStatus.HEALTHY)
			return ServiceHealth.Healthy;
		if(status == HealthStatus.UNHEALTHY)
			return ServiceHealth.Unhealthy;
		if(status == HealthStatus.UNKNOWN)
			return ServiceHealth.Unknown;
		else
			throw new ArgumentOutOfRangeException(nameof(status), status, null);
	}

	public async Task RegisterService(string service, ServiceProperties properties, CancellationToken cancellationToken) {
		var endpoint = new Uri(properties.Endpoint);
		var hostEntry = await Dns.GetHostEntryAsync(endpoint.Host, cancellationToken);
		
		string ipv4Address = hostEntry.AddressList.FirstOrDefault(
			c => c.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
		)!.ToString();
		if(this._environment.IsDevelopment()) {
			ipv4Address = "8.8.8.8";
		}
		
		this._logger.LogInformation($"Resolved {endpoint} to {hostEntry.AddressList.Length} addresses; using {ipv4Address}");
		var creatorId = Guid.NewGuid();
		var registerResponse = await this._serviceDiscoveryClient.RegisterInstanceAsync(new RegisterInstanceRequest() {
			Attributes = new(
				new KeyValuePair<string, string>[] {
					new("ENDPOINT", properties.Endpoint),
					new("AWS_INSTANCE_PORT", endpoint.Port.ToString()),
					new("AWS_INSTANCE_CNAME", endpoint.Host),
					new("AWS_INSTANCE_IPV4", ipv4Address),
					new("TRADIE_ENVIRONMENT", this._environment.EnvironmentName),
				}.Concat(properties.CustomAttributes)
			),
			ServiceId = service,
			InstanceId = properties.InstanceId,
			CreatorRequestId = creatorId.ToString(),
		}, cancellationToken);

		await WaitForSuccess(registerResponse.OperationId, cancellationToken);
		
		this._logger.LogInformation($"Registered instance of {service} with instance ID {properties.InstanceId} at {properties.Endpoint}");
	}

	public async Task DeregisterService(string service, string instanceId, CancellationToken cancellationToken) {
		var resp = await this._serviceDiscoveryClient.DeregisterInstanceAsync(new DeregisterInstanceRequest() {
			ServiceId = service,
			InstanceId = instanceId
		}, cancellationToken);

		await WaitForSuccess(resp.OperationId, cancellationToken);

		this._logger.LogInformation($"Deregistered instance of {service} with instance ID {instanceId}.");
	}

	private async Task WaitForSuccess(string operationId, CancellationToken cancellationToken) {
		GetOperationResponse resp;
		do {
			var req = new GetOperationRequest() {
				OperationId = operationId
			};
			resp = await this._serviceDiscoveryClient.GetOperationAsync(req, cancellationToken);
			if(resp.Operation.Status == OperationStatus.PENDING)
				await Task.Delay(1000, cancellationToken);
		} while(resp.Operation.Status == OperationStatus.PENDING || resp.Operation.Status == OperationStatus.SUBMITTED);

		if(resp.Operation.Status == OperationStatus.FAIL) {
			throw new ApplicationException($"Failed to register or deregister indexer for environment {TradieConfig.League}.");
		}
	}
	
	private readonly IAmazonServiceDiscovery _serviceDiscoveryClient;
	private readonly ILogger<CloudMapServiceRegistry> _logger;
	private readonly IHostEnvironment _environment;
}