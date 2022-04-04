using Amazon.S3;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.Indexer.Pricing;
using Tradie.ItemLog;

namespace Tradie.Indexer; 
#if false
public class IndexTest {
	static Random rng = new Random();
	private ModKey[] SearchMods = Array.Empty<ModKey>();
	Item maxItem;
	float maxSum;
	private bool loadedAllItems = false;

	//byte numQueries = 2;
	//ushort numMods = 1000;
	int numTimesToScan = 100;

	ScanStats stats;

	private int rowsLoaded = 0;

	private IPricingService _pricingService;

	public IndexTest() {
		var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		var s3Client = new AmazonS3Client();
		var cache = new S3PriceCache(s3Client, loggerFactory.CreateLogger<S3PriceCache>());
		var ninjaClient = new NinjaApiClient(loggerFactory.CreateLogger<NinjaApiClient>());
		this._pricingService =
			new NinjaPricingService(cache, ninjaClient, loggerFactory.CreateLogger<NinjaPricingService>());
	}

	private async ValueTask<Item> ConvertToIndexedItem(ItemAnalysis item) {
		/*if(this.loadedAllItems) {
			return null;
		}
		if(!await items.MoveNextAsync()) {
			Console.WriteLine("Reached end of Postgres ItemLog.");
			this.loadedAllItems = true;
			return null;
		}

		this.rowsLoaded++;
		if((this.rowsLoaded % 100000) == 0) {
			Console.WriteLine("Loaded {0} rows.", this.rowsLoaded);
		}
		var curr = items.Current;*/
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

	public async Task SearchByBruteForce(IItemLog itemLog) {
		var sw = Stopwatch.StartNew();
		var items = await itemLog.GetItems(ItemLogOffset.Start, CancellationToken.None)
			.SelectMany(c => c.StashTab.Items.ToAsyncEnumerable())
			.SelectAwait(ConvertToIndexedItem)
			.ToArrayAsync();
			//.GetAsyncEnumerator(CancellationToken.None);

			
		/*var items = new List<Item>();
		while(true) {
			var next = await GetNextItem(itemIterator);
			if(!next.HasValue) {
				break;
			}
			items.Add(next.Value);
		}*/
		
		Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds to generate {items.Length} items.");
		
		sw.Restart();


		foreach(var item in items) {
			stats.itemsScanned++;
			float sum = 0;
			foreach(var affix in item.Affixes) {
				if(this.SearchMods.Contains(affix.Modifier)) {
					sum += affix.Value;
				}
			}
			
			if (sum > maxSum) {
				maxSum = sum;
				maxItem = item;
			}
		}

		Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds to search items.");
	}
	
	public void ResetStats() {
		stats.blocksScanned = 0;
		stats.blocksSkipped = 0;
		stats.itemsScanned = 0;
		this.stats.itemsLoaded = 0;
	}
	
	public void PrintResults() {
		Console.WriteLine(maxItem);
		Console.WriteLine(JsonSerializer.Serialize(this.stats, new JsonSerializerOptions() {
			WriteIndented = true,
			IncludeFields = true,
		}));
		Console.WriteLine(@"
			Blocks Scanned: {0}
			Blocks Skipped: {1}
			Blocks Populated: {2}
			Items Scanned: {3} ({4} blocks)
			Items Skipped: {5}
			Item Blocks Loaded: {6}
			Rows Loaded: {7}
		", stats.blocksScanned, stats.blocksSkipped, stats.populatedBlocks, stats.itemsScanned, stats.itemsScanned / AffixBlock.ItemsPerBlock, -1, this.stats.itemsLoaded, this.rowsLoaded);
	}
	
	public async Task SearchByBlocks(IItemLog itemLog) {
		/*if(itemCapacity < itemsToAllocate) {
			throw new ArgumentOutOfRangeException("itemCapacity");
		}*/
		var sw = Stopwatch.StartNew();
		
		var itemIterator = itemLog.GetItems(ItemLogOffset.Start, CancellationToken.None)
			.SelectMany(c => c.StashTab.Items.ToAsyncEnumerable())
			.SelectAwait(ConvertToIndexedItem)
			.OrderBy(c=>c.ChaosEquivalentPrice)
			.GetAsyncEnumerator(CancellationToken.None);

	    var root = new AffixBlock(BlockKind.Node, null);
	    
	    await this.PopulateBlock(root, itemIterator, 0);

	    Console.WriteLine("Took {0} milliseconds to generate {1} items.", sw.ElapsedMilliseconds, this.rowsLoaded);

	    //auto rootSum = root.ranges.get(getBlockKey(searchMod1, searchKind), AffixRange.init).maxValue 
	    //    + root.ranges.get(getBlockKey(searchMod2, searchKind), AffixRange.init).maxValue;

	    int totalMS = 0;
	    for(int i = 0; i < numTimesToScan; i++) {
	        maxItem = default;
	        maxSum = 0;
	        
	        this.PrepareSearchMods();
	        
	        sw.Restart();
	        this.SearchInBlock(root);
	        Console.WriteLine("Took {0} milliseconds to search for items for mods {1}.", sw.ElapsedMilliseconds, SearchMods);
	        totalMS += (int)sw.ElapsedMilliseconds;
	        this.PrintResults();
	        var rootSum = this.GetSum(root);
	        Console.WriteLine("Max possible sum is {0}, and received sum is {1}", rootSum, maxSum);
	        
	        maxItem = default;
	        this.ResetStats();
	    }

	    Console.WriteLine("Average search time was {0}", totalMS / numTimesToScan);
	    //Console.ReadLine();
	}

	float GetSum(AffixBlock? block) {
		if(block == null) {
			return 0;
		}
	    float sum = 0;
	    foreach(var mod in SearchMods) {
		    sum += block.Ranges.Get(mod).MaxValue;
	    }
	    return sum;
	}

	void SearchInBlock(AffixBlock block) {
	    //auto blockSum = block.ranges.get(getBlockKey(searchMod1, searchKind), AffixRange.init).maxValue 
	    //    + block.ranges.get(getBlockKey(searchMod2, searchKind), AffixRange.init).maxValue;
	    float blockSum = GetSum(block);

	    if(blockSum < maxSum) {
	        stats.blocksSkipped++;
	        return;
		}
	    stats.blocksScanned++;
	    if(block.Kind == BlockKind.Leaf) {
		    this.stats.itemsLoaded++;
	        foreach(var item in block.Items!) {
	            stats.itemsScanned++;
	            float sum = 0;
	            foreach(var affix in item.Affixes) {
		            if(this.SearchMods.Contains(affix.Modifier)) {
			            sum += affix.Value;
		            }
	            }
	            
	            if (sum > maxSum) {
	                maxSum = sum;
	                maxItem = item;
	            }
	        }
		} else {
	        foreach(var child in block.Blocks!) {
	            this.SearchInBlock(child);
		    }
	    }
	}

	async ValueTask PopulateBlock(AffixBlock block, IAsyncEnumerator<Item> items, int depth) {
		if(this.loadedAllItems) {
			return;
		}
		/*if(itemsAllocated > itemsToAllocate) {
	        return;
		}*/
	    if(depth == AffixBlock.MaxDepth) {
		    if(this.loadedAllItems) {
			    return;
		    }
		    for(int i = 0; i < block.Items!.Length; i++) {
			    if(!await items.MoveNextAsync()) {
				    Console.WriteLine("Reached end of Postgres ItemLog.");
				    this.loadedAllItems = true;
				    return;
			    }
			    this.rowsLoaded++;
			    if(this.rowsLoaded % 100000 == 0) {
				    Console.WriteLine("Loaded {0} rows.", this.rowsLoaded);
			    }

			    var nextItem = items.Current;
			    block.Items[i] = nextItem;

			    foreach(var affix in nextItem.Affixes) {
				    UpdateBlocks(block, affix);
			    }
		    }
	        
	        stats.populatedBlocks++; 
	        
	        if((this.rowsLoaded % 100000) == 0) {
		        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
		        long endBytes = currentProcess.WorkingSet64;
	            Console.WriteLine("Finished allocating {0} items using {1}MB of RAM.", this.rowsLoaded, endBytes / (1024 * 1024));
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
	
	void PrepareSearchMods() {
		//searchMods.Clear();
		this.SearchMods = Array.Empty<ModKey>();
		//searchMods = [];
		//this.SearchMods = Enumerable.Range(0, this.numQueries).Select(_ => (ushort)rng.Next(1, this.numMods + 1))
		//	.ToArray();
		this.SearchMods = new ModKey[] {
			new(9024037547368883040, ModKind.Explicit),
			new(9119717384377170561, ModKind.Explicit),
			new(14277321269326717293, ModKind.Explicit)
		};
	}
}

public struct ScanStats {
	public int itemsScanned;
	public int blocksScanned;
	public int blocksSkipped;
	public int populatedBlocks;
	public int itemsLoaded;
};

/*public class SortedAffixRangeList {
	public static SortedAffixRangeList Empty() {
		return new SortedAffixRangeList() {
			elements = new Dictionary<uint, AffixRange>()
		};
	}

	public AffixRange Get(uint searchKey) {
		//return ref CollectionsMarshal.GetValueRefOrNullRef(this.elements, searchKey);
		if(this.elements.TryGetValue(searchKey, out var res))
			return res;
		return default;
	}

	public ref AffixRange GetWithAddingDefault(uint searchKey, out bool exists) {
		return ref CollectionsMarshal.GetValueRefOrAddDefault(this.elements, searchKey, out exists);
	}

	private Dictionary<uint, AffixRange> elements = new Dictionary<uint, AffixRange>();
}*/
#endif