using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tradie.Common;
using Tradie.Common.Services;

namespace Tradie.Indexer;

public class CloudMapRegistrationService : IHostedService {
	private static readonly string InstanceId = $"indexer-{TradieConfig.League}";
	
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
	}
	
	public async Task StartAsync(CancellationToken cancellationToken) {
		await this._serviceRegistry.RegisterService(
			TradieConfig.DiscoveryServiceIndexerId,
			new(
				this._listenAddress,
				InstanceId,
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
			InstanceId,
			cancellationToken
		);
	}

	private readonly IServiceRegistry _serviceRegistry;
	private readonly string _listenAddress;
	private readonly ILogger<CloudMapRegistrationService> _logger;
	private readonly IHostEnvironment _hostEnvironment;
}