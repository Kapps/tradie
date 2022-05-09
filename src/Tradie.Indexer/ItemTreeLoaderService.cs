using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Indexer.Pricing;
using Tradie.Indexer.Storage;
using Tradie.ItemLog;

namespace Tradie.Indexer;

/// <summary>
/// A service used to load all items into an ItemTree to begin searching requests.
/// </summary>
public class ItemTreeLoaderService : IHostedService {
	public ItemTreeLoaderService(ItemTree itemTree, IItemLog itemLog, IPricingService pricingService) {
		this._itemLog = itemLog;
		this._itemTree = itemTree;
		this._pricingService = pricingService;
	}

	public async Task StartAsync(CancellationToken cancellationToken) {
		Console.WriteLine("Populating item tree...");
		var sw = Stopwatch.StartNew();
		int numPopulated = 0;

		var cts = new CancellationTokenSource();
		await foreach(var record in this._itemLog.GetItems(ItemLogOffset.Start, cts.Token).WithCancellation(cts.Token)) {
			foreach(var analyzedItem in record.StashTab.Items) {
				var converted = await ConvertToIndexedItem(analyzedItem);
				this._itemTree.Add(converted);
				numPopulated++;
				if(numPopulated % 1000 == 0) {
					Console.WriteLine($"Finished populating {numPopulated} entries.");
				}
			}
		}
		Console.WriteLine($"Took {sw.Elapsed} to populate the item tree with {this._itemTree.Count} entries.");
	}

	public Task StopAsync(CancellationToken cancellationToken) {
		return Task.CompletedTask;
	}
	
	private async ValueTask<Item> ConvertToIndexedItem(ItemAnalysis item) {
		var mods = item.GetRequired<ItemAffixesAnalysis>(KnownAnalyzers.Modifiers);
		var tradeAttributes = item.GetRequired<TradeListingAnalysis>(KnownAnalyzers.TradeAttributes);
		var affixes = mods.Affixes.Select(c => new Affix(new ModKey(c.Hash, c.Kind), (float)c.Scalar))
			.OrderBy(c => c.Modifier.ModHash).ThenBy(c=>(int)c.Modifier.Kind)
			.ToArray();
		var rawPrice = tradeAttributes.Price.GetValueOrDefault();
		float calculatedPrice = await this._pricingService.GetChaosEquivalentPrice(rawPrice, CancellationToken.None);
		if(calculatedPrice == 0) {
			calculatedPrice = float.MaxValue;
		}
		return new Item(item.ItemId, calculatedPrice, affixes);
	}

	private readonly ItemTree _itemTree;
	private readonly IItemLog _itemLog;
	private readonly IPricingService _pricingService;
}