using Nest;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;

namespace Tradie.ItemLogBuilder.Elastic.Models;

/// <summary>
/// Representation of a single item analysis for an Elastic database.
/// </summary>
public record LoggedItem(
	string ItemId,
	[property:Nested] ItemTypeAnalysis? ItemType,
	[property:Nested] ItemAffixesAnalysis? Affixes,
	[property:Nested] ItemDetailsAnalysis? Details,
	[property:Nested] TradeListingAnalysis? TradeListing
);
