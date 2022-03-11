using Tradie.Analyzer.Models;

namespace Tradie.Indexer.Pricing;

/// <summary>
/// Represents the cost of a currency item, relative to chaos orbs.
/// </summary>
/// <param name="Currency">The currency item.</param>
/// <param name="ChaosEquivalentCost">Its cost in chaos orbs.</param>
public record struct CurrencyPrice(
	Currency Currency,
	float ChaosEquivalentCost
);
