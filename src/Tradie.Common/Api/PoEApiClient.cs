using RateLimiter;

namespace Tradie.Common;

/// <summary>
/// An ApiClient used to make calls to the PoE data apis.
/// </summary>
public class PoEApiClient : ApiClient {
	public const string PoEBaseUrl = "https://www.pathofexile.com/api/";

	public PoEApiClient() : base(TimeLimiter.GetFromMaxCountByInterval(2, TimeSpan.FromSeconds(1)), PoEBaseUrl) {
		
	}
}
