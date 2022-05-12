using MessagePack;
using System.Runtime.Serialization;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Analyzers;

/// <summary>
/// An Item Analyzer that extracts out basic properties from the raw item and analyzes the trade aspects, such as price.
/// </summary>
public class TradePropertiesAnalyzer : IItemAnalyzer {
	public static ushort Id { get; } = (ushort)KnownAnalyzers.TradeAttributes;

	public ValueTask AnalyzeItems(AnalyzedItem[] items) {
		foreach(var item in items) {
			var raw = item.RawItem;
			var analysis = item.Analysis;
			ItemPrice.TryParse(raw.Note, out var price); // Default if false is desired.
			analysis.PushAnalysis(Id, new TradeListingAnalysis(raw.X, raw.Y, price, raw.Note));
		}

		return ValueTask.CompletedTask;
	}
	
	public ValueTask DisposeAsync() {
		return ValueTask.CompletedTask;
	}
}

/// <summary>
/// Analyzed properties on an item related to trading for it.
/// </summary>
/// <param name="X">X Location within the stash tab.</param>
/// <param name="Y">Y Location within the stash tab.</param>
/// <param name="Price">The amount of currency the item is listed for.</param>
/// <param name="Note">The actual note listed, before parsing.</param>
[DataContract, MessagePackObject]
public readonly record struct TradeListingAnalysis (
	[property:DataMember, Key(0)] ushort X,
	[property:DataMember, Key(1)] ushort Y,
	[property:DataMember, Key(2)] ItemPrice? Price,
	[property:DataMember, Key(3)] string? Note
) : IAnalyzedProperties;