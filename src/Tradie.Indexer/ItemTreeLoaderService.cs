using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Common;
using Tradie.Indexer.Pricing;
using Tradie.Indexer.Storage;
using Tradie.ItemLog;

namespace Tradie.Indexer;

/// <summary>
/// A service used to load all items into an ItemTree to begin searching requests.
/// As new items are added to the log, the service will periodically update the ItemTree.
/// </summary>
public class ItemTreeLoaderService : IHostedService {
	public ItemTreeLoaderService(ItemTree itemTree, IItemLog itemLog, IPricingService pricingService) {
		this._itemLog = itemLog;
		this._itemTree = itemTree;
		this._pricingService = pricingService;
		this._updateCts = new();
	}

	public async Task StartAsync(CancellationToken cancellationToken) {
		Console.WriteLine("Populating item tree...");
		var sw = Stopwatch.StartNew();
		var items = GetItemIterator(cancellationToken);

		await this._itemTree.AddBulk(items, cancellationToken);

		Console.WriteLine($"Took {sw.Elapsed} to populate the item tree with {this._itemTree.Count} entries.");
		
		this.StartBackgroundUpdates();
	}
	
	private async void StartBackgroundUpdates() {
		while(!this._updateCts.IsCancellationRequested) {
			var sw = Stopwatch.StartNew();
			var items = this._itemLog.GetItems(this._latestRecord.Offset, this._updateCts.Token)
				.SelectMany(c =>
					c.StashTab.Items.Select(c => ConvertToIndexedItem(c).GetAwaiter().GetResult()).ToAsyncEnumerable());

			Console.WriteLine($"Performing background item update; started with {this._itemTree.Count} entries.");
			
			await this._itemTree.AddBulk(items, this._updateCts.Token);
			
			Console.WriteLine($"Took {sw.Elapsed} to update the item tree with {this._itemTree.Count} entries afterwards.");

			await Task.Delay(TimeSpan.FromSeconds(10));
		}
	}

	private IAsyncEnumerable<Item> GetItemIterator(CancellationToken cancellationToken) {
		return this._itemLog.GetItems(this._latestRecord.Offset, cancellationToken)
			.WithCompletionCallback(record => {
				this._latestRecord = record;
				return Task.CompletedTask;
			})
			.SelectMany(c =>
				c.StashTab.Items.Select(c => ConvertToIndexedItem(c).GetAwaiter().GetResult()).ToAsyncEnumerable());
	}

	public Task StopAsync(CancellationToken cancellationToken) {
		this._updateCts.Cancel();
		return Task.CompletedTask;
	}
	
	private async ValueTask<Item> ConvertToIndexedItem(ItemAnalysis item) {
		var mods = item.GetRequired<ItemAffixesAnalysis>(KnownAnalyzers.Modifiers);
		var tradeAttributes = item.GetRequired<TradeListingAnalysis>(KnownAnalyzers.TradeAttributes);
		Array.Sort(mods.Affixes);
		var affixes = new Affix[mods.Affixes.Length];
		for(int i = 0; i < affixes.Length; i++) {
			var orig = mods.Affixes[i];
			affixes[i] = new Affix(new ModKey(orig.Hash, orig.Kind), (float)orig.Scalar);
		}

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
	private readonly CancellationTokenSource _updateCts;
	private LogRecord _latestRecord;
}