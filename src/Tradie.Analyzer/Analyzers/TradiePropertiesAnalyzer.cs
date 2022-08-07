using MessagePack;
using System.Runtime.Serialization;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;

namespace Tradie.Analyzer.Analyzers;

/// <summary>
/// An Item Analyzer that extracts out basic properties from the raw item and analyzes the trade aspects, such as price.
/// </summary>
public class TradePropertiesAnalyzer : IItemAnalyzer {
	public TradePropertiesAnalyzer(IPriceHistoryRepository priceHistoryRepository) {
		this._priceHistoryRepository = priceHistoryRepository;
	}

	public int Order => 100;
	
	public static ushort Id { get; } = KnownAnalyzers.TradeAttributes;

	public async ValueTask AnalyzeItems(AnalyzedItem[] items) {
		if(items.Length == 0) {
			return;
		}
		
		var priceHistories = (await this._priceHistoryRepository.LoadLatestPricesForItems(
			items.Select(c => c.Id),
			CancellationToken.None
		)).ToDictionary(c=>c.ItemId);
		
		var updatedHistories = new List<ItemPriceHistory>();
		
		foreach(var item in items) {
			var raw = item.RawItem;
			var analysis = item.Analysis;
			if(!ItemPrice.TryParse(raw.Note, out var price)) // Default if false is desired.
				price = ItemPrice.None;
			analysis.PushAnalysis(Id, new TradeListingAnalysis(raw.X, raw.Y, price, raw.Note));

			if(!priceHistories.TryGetValue(item.Id, out var history) || history.Price != price) {
				updatedHistories.Add(new ItemPriceHistory(item.Id, price, DateTime.Now));
			}
		}
		
		if(updatedHistories.Count > 0) {
			await this._priceHistoryRepository.RecordPriceHistories(updatedHistories, CancellationToken.None);
		}
	}
	
	public ValueTask DisposeAsync() {
		return ValueTask.CompletedTask;
	}

	private readonly IPriceHistoryRepository _priceHistoryRepository;
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