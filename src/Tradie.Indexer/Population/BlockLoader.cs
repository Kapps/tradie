using Microsoft.Extensions.Logging;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Indexer.Pricing;
using Tradie.ItemLog;

namespace Tradie.Indexer.Population;

#if false
public interface IBlockLoader {
	Task AppendFromItemLog(AffixBlock block, IItemLog log, CancellationToken cancellationToken);
}

public class BlockLoader : IBlockLoader {
	public BlockLoader(IPricingService pricingService, ILogger<BlockLoader> logger) {
		this._pricingService = pricingService;
		this._logger = logger;
	}
	
	public Task AppendFromItemLog(AffixBlock block, IItemLog log, CancellationToken cancellationToken) {
		throw new NotImplementedException();
	}
	
	private async ValueTask<Item> ConvertToIndexedItem(ItemAnalysis item) {
		this._rowsLoaded++;
		if((this._rowsLoaded % 100000) == 0) {
			Console.WriteLine("Loaded {0} rows.", this._rowsLoaded);
		}
		
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
	
	async ValueTask PopulateBlock(AffixBlock block, IAsyncEnumerator<Item> items, int depth) {
		/*if(itemsAllocated > itemsToAllocate) {
	        return;
		}*/
	    if(depth == AffixBlock.MaxDepth) {
		    for(int i = 0; i < block.Items!.Length; i++) {
			    if(!await items.MoveNextAsync()) {
				    Console.WriteLine("Reached end of Postgres ItemLog.");
				    return;
			    }
			  
			    var nextItem = items.Current;
			    block.Items[i] = nextItem;

			    foreach(var affix in nextItem.Affixes) {
				    UpdateBlocks(block, affix);
			    }
		    }
	    } else {
		    for(int i = 0; i < block.Blocks!.Length; i++) {
			    if(this.loadedAllItems) {
				    return;
			    }
			    var child = new AffixBlock((depth + 1) == AffixBlock.MaxDepth ? BlockKind.Leaf : BlockKind.Node, block);
			    block.Blocks[i] = child;
			    await this.PopulateBlock(child, items, depth + 1);
		    }
	    }
	}

	void UpdateBlocks(AffixBlock? block, Affix affix) {
		if(block == null) return;

		var key = affix.Modifier; 
		ref var range = ref block.Ranges.GetWithAddingDefault(key, out var exists);
		if(!exists) {
			range.MinValue = range.MaxValue = affix.Value;
			range.Key = key;
	        
	        UpdateBlocks(block.Parent, affix);
	    } else {
			if(affix.Value < range.MinValue) {
	            range.MinValue  = affix.Value;
	            UpdateBlocks(block.Parent, affix);
			}
	        if(affix.Value > range.MaxValue) {
	            range.MaxValue = affix.Value;
	            UpdateBlocks(block.Parent, affix);
			}
		}
	}

	private int _rowsLoaded = 0;
	private readonly IPricingService _pricingService;
	private readonly ILogger<BlockLoader> _logger;
}
#endif