﻿using Microsoft.Extensions.Logging;
using Tradie.Analyzer.Models;

namespace Tradie.Indexer.Pricing;

/// <summary>
/// A service to support calculating normalized prices from currency listings.
/// </summary>
public interface IPricingService {
	/// <summary>
	/// Gets the equivalent normalized price (i.e. equivalent price in chaos orbs) for an ItemPrice. 
	/// </summary>
	ValueTask<float> GetChaosEquivalentPrice(ItemPrice price, CancellationToken cancellationToken);
}

/// <summary>
/// An IPricingService implementation that queries the poe.ninja API for equivalent prices.
/// </summary>
public class NinjaPricingService : IPricingService {
	public NinjaPricingService(IPriceCache priceCache, INinjaApi ninjaClient, ILogger<NinjaPricingService> logger) {
		this._ninjaClient = ninjaClient;
		this._logger = logger;
		this._priceCache = priceCache;
	}
	
	public async ValueTask<float> GetChaosEquivalentPrice(ItemPrice price, CancellationToken cancellationToken) {
		if(this._priceList == null) {
			await this.PopulateValues(cancellationToken);
		}

		return this._priceList![(int)price.Currency] * price.Amount;
	}

	private async Task PopulateValues(CancellationToken cancellationToken) {
		var cachedValues = await this._priceCache.LoadCachedValues(cancellationToken);
		if(cachedValues != null) {
			AssignPrices(cachedValues);
			this._logger.LogInformation("Assigned prices from existing cached values");
			return;
		}

		var remoteValues = await this.GetRemoteValues(cancellationToken);
		AssignPrices(remoteValues);
		this._logger.LogInformation("Assigned prices from remote data source");
	}

	private void AssignPrices(IEnumerable<CurrencyPrice> prices) {
		var priceMap = prices.ToDictionary(c => c.Currency, c => c.ChaosEquivalentCost);
		this._priceList = new float[Enum.GetValues<Currency>().Length];
		
		foreach(var currency in Enum.GetValues<Currency>().Where(c=>c != Currency.None)) {
			var mapped = priceMap[currency];
			this._priceList[(int)currency] = mapped;
		}
	}

	private async Task<IEnumerable<CurrencyPrice>> GetRemoteValues(CancellationToken cancellationToken) {
		this._logger.LogInformation("Loading pricing information from poe.ninja");

		var results = new List<CurrencyPrice>();
		var listings = await this._ninjaClient.GetPriceListings(cancellationToken);
		foreach(var listing in listings) {
			if(ItemPrice.TryParseCurrency(listing.TradeId, out var currency)) {
				results.Add(new CurrencyPrice(currency, listing.ChaosEquivalent));
			}
		}

		return results;
	}

	private float[]? _priceList;
	private readonly IPriceCache _priceCache;
	private readonly INinjaApi _ninjaClient;
	private readonly ILogger<NinjaPricingService> _logger;
}