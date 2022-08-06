using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tradie.Common;
using Tradie.Common.Services;

namespace Tradie.Indexer;

public class CloudMapRegistrationService : IHostedService {
	public CloudMapRegistrationService(
		IServiceRegistry serviceRegistry,
		string listenAddress,
		ILogger<CloudMapRegistrationService> logger,
		IHostEnvironment hostEnvironment
	) {
		this._serviceRegistry = serviceRegistry;
		this._listenAddress = listenAddress;
		this._logger = logger;
		this._hostEnvironment = hostEnvironment;
		this._instanceId = $"indexer-{TradieConfig.League}-{this._hostEnvironment.EnvironmentName}";

	}
	
	public async Task StartAsync(CancellationToken cancellationToken) {
		await this._serviceRegistry.RegisterService(
			TradieConfig.DiscoveryServiceIndexerId,
			new(
				this._listenAddress,
				this._instanceId,
				new Dictionary<string, string>() {
					{ "TRADIE_LEAGUE", TradieConfig.League! }
				}
			),
			cancellationToken
		);
	}

	public async Task StopAsync(CancellationToken cancellationToken) {
		await this._serviceRegistry.DeregisterService(
			TradieConfig.DiscoveryServiceIndexerId,
			this._instanceId,
			cancellationToken
		);
	}

	private readonly IServiceRegistry _serviceRegistry;
	private readonly string _listenAddress;
	private readonly ILogger<CloudMapRegistrationService> _logger;
	private readonly IHostEnvironment _hostEnvironment;
	private readonly string _instanceId;
}