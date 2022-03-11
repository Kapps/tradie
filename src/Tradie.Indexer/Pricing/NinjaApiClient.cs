using Microsoft.Extensions.Logging;
using RateLimiter;
using System.Data;
using System.Text.Json;
using Tradie.Analyzer.Models;
using Tradie.Common;

namespace Tradie.Indexer.Pricing;

/// <summary>
/// Allows querying the poe.ninja API for resources such as currency equivalencies.
/// </summary>
public interface INinjaApi {
	/// <summary>
	/// Returns the list of currencies and price equivalencies from the remote API.
	/// </summary>
	Task<NinjaPrice[]> GetPriceListings(CancellationToken cancellationToken);
}

/// <summary>
/// An ApiClient implementation for accessing the poe.ninja API.
/// </summary>
public class NinjaApiClient : ApiClient, INinjaApi {
	public const string NinjaBaseUrl = "https://poe.ninja/api/";

	public NinjaApiClient(ILogger<NinjaApiClient> logger) : base(TimeLimiter.GetFromMaxCountByInterval(2, TimeSpan.FromSeconds(2)), NinjaBaseUrl) {
		this._logger = logger;
	}

	public async Task<NinjaPrice[]> GetPriceListings(CancellationToken cancellationToken) {
		string json = await GetNinjaPriceJson(cancellationToken);
		
		dynamic obj = JsonSerializer.Deserialize<dynamic>(json) ?? throw new NoNullAllowedException();
		
		Dictionary<string, NinjaCurrency> ninjaNameToCurrency = new();
		foreach(dynamic detail in obj.currencyDetails) {
			string name = detail.name;
			string tradeId = detail.tradeId;
			ninjaNameToCurrency[name] = new NinjaCurrency(tradeId, name);
		}

		List<NinjaPrice> prices = new();
		foreach(dynamic line in obj.lines) {
			string typeName = line.currencyTypeName;
			if(ninjaNameToCurrency.TryGetValue(typeName, out var currency)) {
				float chaosEquiv = line.chaosEquivalent;
				prices.Add(new NinjaPrice(currency.TradeId, typeName, chaosEquiv));
			}
		}

		return prices.ToArray();
	}
	
	private async Task<string> GetNinjaPriceJson(CancellationToken cancellationToken) {
		try {
			string resp = await this.Get(
				"data/CurrencyOverview",
				("league", TradieConfig.League ?? throw new ArgumentNullException()),
				("type", "Currency"),
				("language", "en")
			);
			return resp;
		} catch(Exception e) {
			this._logger.LogError("Failed to load currency values from poe.ninja: {Exception}", e);
			throw;
		}
	}
	
	private record struct NinjaCurrency(string TradeId, string TypeName);

	private readonly IApiClient _apiClient;
	private readonly ILogger<NinjaApiClient> _logger;
}



public record struct NinjaPrice(string TradeId, string TypeName, float ChaosEquivalent);