using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tradie.Common.Services;

/// <summary>
/// Allows for acquiring and caching GRPC channels to registered services. 
/// </summary>
public interface IGrpcServicePool {
	/// <summary>
	/// Gets or creates a channel to an instance of the given service that has the specified attributes. 
	/// </summary>
	Task<GrpcChannel> GetChannelForService(string serviceId, Dictionary<string, string> requiredAttributes,
		CancellationToken cancellationToken);
}

public class GrpcServicePool : IGrpcServicePool {
	public GrpcServicePool(IServiceRegistry serviceRegistry, ILogger<GrpcServicePool> logger) {
		AppContext.SetSwitch(
			"System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
		this._serviceRegistry = serviceRegistry;
		this._logger = logger;
		this._cache = new(new MemoryCacheOptions());
	}

	public async Task<GrpcChannel> GetChannelForService(string serviceId, Dictionary<string, string> requiredAttributes, CancellationToken cancellationToken) {
		string cacheKey = this.GetCacheKey(serviceId, requiredAttributes);
		
		bool recreateChannel = false;
		var channel = this._cache.Get<GrpcChannel>(cacheKey);
		
		/*if(channel != null && (channel.State == ConnectivityState.TransientFailure || channel.State == ConnectivityState.Shutdown)) {
			recreateChannel = true;
			this._logger.LogWarning(
				"Channel for service {serviceId} is in a {state} state. Recreating channel.",
				serviceId, channel.State
			);
		}*/
		
		if (channel == null || recreateChannel) {
			channel = await this.CreateChannel(serviceId, requiredAttributes, cancellationToken);
			this._cache.Set(cacheKey, channel, new MemoryCacheEntryOptions {
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
				PostEvictionCallbacks = {
					new PostEvictionCallbackRegistration() {
						EvictionCallback = (_, value, _, _) => ((GrpcChannel)value).Dispose()
					}
				}
			});
		}

		return channel;
	}

	private async Task<GrpcChannel> CreateChannel(
		string serviceId, 
		Dictionary<string,string> requiredAttributes,
		CancellationToken cancellationToken
	) {
		var service = await this._serviceRegistry.DiscoverService(serviceId, requiredAttributes, cancellationToken);
		
		var channel = GrpcChannel.ForAddress(service.Endpoint, new GrpcChannelOptions() {
			HttpHandler = new HttpClientHandler() {
				// ಠ_ಠ
				ServerCertificateCustomValidationCallback = (_, _, _, _) => true
			}
		});
		
		return channel;
	}

	private string GetCacheKey(string serviceId, Dictionary<string, string> requiredAttributes) {
		var req = new {
			ServiceId = serviceId,
			RequiredAttributes = requiredAttributes
		};
		
		string cacheKey = SpanJson.JsonSerializer.Generic.Utf16.Serialize(req);
		return cacheKey;
	}

	private readonly IServiceRegistry _serviceRegistry;
	private readonly ILogger<GrpcServicePool> _logger;
	private readonly MemoryCache _cache;
}