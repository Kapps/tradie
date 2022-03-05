using System.Diagnostics;
using System.Text.Json;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.ItemLog;

namespace Tradie.Indexer; 

public class IndexTest {
	public const int itemsPerBlock = 16;
	public const int blocksPerBlock = 10;
	public const int maxDepth = 7;
	//public const int itemsToAllocate = 25_000_000;
	public static readonly int itemCapacity = (int)Math.Pow(blocksPerBlock, maxDepth) * itemsPerBlock;

	
	static Random rng = new Random();
	

	//int itemsAllocated;
 

	//byte searchKind = 1;

	//private ushort[] searchMods = Array.Empty<ushort>();
	private ulong[] searchMods = Array.Empty<ulong>();
	//HashSet<ushort> searchMods = new HashSet<ushort>();
	Item maxItem;
	float maxSum;

	//byte numQueries = 2;
	//ushort numMods = 1000;
	//int numTimesToScan = 100;

	ScanStats stats;

	private int rowsLoaded = 0;
	uint getBlockKey(ushort modifier, byte kind) {
		return (uint)(modifier << 16) | kind;
	}

	private async ValueTask<Item?> GetNextItem(IAsyncEnumerator<ItemAnalysis> items) {
		if(!await items.MoveNextAsync()) {
			Console.WriteLine("Reached end of Postgres ItemLog.");
			return null;
		}

		this.rowsLoaded++;
		if((this.rowsLoaded % 10000) == 0) {
			Console.WriteLine("Loaded {0} rows.", this.rowsLoaded);
		}
		var curr = items.Current;
		//var details = curr.GetRequired<ItemDetailsAnalysis>(KnownAnalyzers.ItemDetails);
		var mods = curr.GetRequired<ItemAffixesAnalysis>(KnownAnalyzers.Modifiers);
		return new Item(curr.ItemId,
			mods.Affixes.Select(c => new Affix(c.Hash, (float)c.Scalar, (byte)c.Kind)).ToArray());
	}

	/*Item createItem() {
		var affixCount = rng.Next(4, 7);
		var id = Guid.NewGuid();
		var affixes = new Affix[affixCount];
		
		for(int i = 0; i < affixCount; i++) {
			var kind = (byte)rng.Next(1, 4);
			do {
				var modifier = (ushort)rng.Next(1, numMods);
				bool next = false;
				for(int j = 0; j < i; j++) {
					if(affixes[j].Modifier == modifier) {
						next = true;
						break;
					}
				}

				if(next) {
					continue;
				}

				var value = (ushort)rng.Next(1, 101);
				affixes[i] = new Affix(modifier, value, kind);
				break;
			} while(true);
		}
		
		Array.Sort(affixes, (a, b) => a.Modifier.CompareTo(b.Modifier));

		//affixes = affixes.OrderBy(c => c.modifier).ToArray();
		return new Item(id, affixes);
	}*/
	
	
	public async Task searchByBruteForce(IItemLog itemLog) {
		var sw = Stopwatch.StartNew();
		var itemIterator = itemLog.GetItems(ItemLogOffset.Start, CancellationToken.None)
			.SelectMany(c => c.StashTab.Items.ToAsyncEnumerable())
			.GetAsyncEnumerator(CancellationToken.None);
		//var items = new Item[count];

		/*for(int i = 0; i < count; i++) {
			items[i] = this.createItem();
		}*/

		var items = new List<Item>();
		while(true) {
			var next = await GetNextItem(itemIterator);
			if(!next.HasValue) {
				break;
			}
			items.Add(next.Value);
		}

		//Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds to generate {itemsToAllocate} items.");
		Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds to generate {items.Count} items.");
		
		sw.Restart();


		foreach(var item in items) {
			stats.itemsScanned++;
			float sum = 0;
			foreach(var affix in item.Affixes) {
				if(this.searchMods.Contains(affix.Modifier)) {
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
		", stats.blocksScanned, stats.blocksSkipped, stats.populatedBlocks, stats.itemsScanned, stats.itemsScanned / itemsPerBlock, -1, this.stats.itemsLoaded);
	}
	
	/*public void SearchByBlocks() {
		if(itemCapacity < itemsToAllocate) {
			throw new ArgumentOutOfRangeException("itemCapacity");
		}
		var sw = Stopwatch.StartNew();

	    Block root = new Block(BlockKind.node, null);
	    
	    this.PopulateBlock(ref root, null, 0);

	    Console.WriteLine("Took {0} milliseconds to generate {1} items.", sw.ElapsedMilliseconds, itemsAllocated);

	    //auto rootSum = root.ranges.get(getBlockKey(searchMod1, searchKind), AffixRange.init).maxValue 
	    //    + root.ranges.get(getBlockKey(searchMod2, searchKind), AffixRange.init).maxValue;

	    int totalMS = 0;
	    for(int i = 0; i < numTimesToScan; i++) {
	        maxItem = default;
	        maxSum = 0;
	        
	        this.PrepareSearchMods();
	        
	        sw.Restart();
	        searchInBlock(root);
	        Console.WriteLine("Took {0} milliseconds to search for items for mods {1}.", sw.ElapsedMilliseconds, searchMods);
	        totalMS += (int)sw.ElapsedMilliseconds;
	        this.PrintResults();
	        var rootSum = getSum(root);
	        Console.WriteLine("Max possible sum is {0}, and received sum is {1}", rootSum, maxSum);
	        
	        maxItem = default;
	        this.ResetStats();
	    }

	    Console.WriteLine("Average search time was {0}", totalMS / numTimesToScan);
	    //Console.ReadLine();
	}

	int getSum(Block? block) {
		if(block == null) {
			return 0;
		}
	    int sum = 0;
	    foreach(var mod in searchMods) {
	        var blockKey = getBlockKey(mod, searchKind);
	        sum += block.ranges.Get(blockKey).maxValue;
	    }
	    return sum;
	}

	void searchInBlock(Block block) {
	    //auto blockSum = block.ranges.get(getBlockKey(searchMod1, searchKind), AffixRange.init).maxValue 
	    //    + block.ranges.get(getBlockKey(searchMod2, searchKind), AffixRange.init).maxValue;
	    int blockSum = getSum(block);

	    if(blockSum < maxSum) {
	        stats.blocksSkipped++;
	        return;
		}
	    stats.blocksScanned++;
	    if(block.kind == BlockKind.leaf) {
		    this.stats.itemsLoaded++;
	        foreach(var item in block.items!) {
	            stats.itemsScanned++;
	            int sum = 0;
	            foreach(var affix in item.Affixes) {
		            if(this.searchMods.Contains(affix.Modifier)) {
			            sum += affix.Value;
		            }
	            }
	            
	            if (sum > maxSum) {
	                maxSum = sum;
	                maxItem = item;
	            }
	        }
		} else {
	        foreach(var child in block.blocks!) {
	            searchInBlock(child);
		    }
	    }
	}

	void PopulateBlock(ref Block block, Block? parent, int depth) {
	    if(itemsAllocated > itemsToAllocate) {
	        return;
		}
	    if(depth == maxDepth) {
		    block = new Block(BlockKind.leaf, parent);

		    for(int i = 0; i < block.items!.Length; i++) {
			    block.items[i] = createItem();

			    foreach(var affix in block.items[i].Affixes) {
				    updateBlocks(block, affix);
			    }
		    }
	        
	        stats.populatedBlocks++; 

	        itemsAllocated += itemsPerBlock;
	        if((itemsAllocated % 100000) == 0) {
		        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
		        long endBytes = currentProcess.WorkingSet64;
	            Console.WriteLine("Finished allocating {0} items using {1}MB of RAM.", itemsAllocated, endBytes / (1024 * 1024));
	        }
		} else {
		    block = new Block(BlockKind.node, parent);

		    for(int i = 0; i < block.blocks!.Length; i++) {
			    this.PopulateBlock(ref block.blocks![i], block, depth + 1);
		    }
	    }
	}

	void updateBlocks(Block? block, Affix affix) {
		if(block == null) return;

		var key = getBlockKey(affix.Modifier, affix.Kind); 
		ref var range = ref block.ranges.GetWithAddingDefault(key, out var exists);
		if(!exists) {
			range.minValue = range.maxValue = affix.Value;
			range.key = key;
	        
	        updateBlocks(block.parent, affix);
	    } else {
			if(affix.Value < range.minValue) {
	            range.minValue  = affix.Value;
	            updateBlocks(block.parent, affix);
			}
	        if(affix.Value > range.maxValue) {
	            range.maxValue = affix.Value;
	            updateBlocks(block.parent, affix);
			}
		}
	}
	
	void PrepareSearchMods() {
		//searchMods.Clear();
		this.searchMods = Array.Empty<ushort>();
		//searchMods = [];
		this.searchMods = Enumerable.Range(0, this.numQueries).Select(_ => (ushort)rng.Next(1, this.numMods + 1))
			.ToArray();
	}*/
	
	
}

public readonly struct Item {
	public readonly string Id;
	public readonly Affix[] Affixes;

	public Item(string id, Affix[] affixes) {
		this.Id = id;
		this.Affixes = affixes;
	}
}

public readonly struct Affix {
	public readonly ulong Modifier;
	public readonly float Value;
	public readonly byte Kind;

	public Affix(ulong hash, float value, byte kind) {
		this.Modifier = hash;
		this.Value = value;
		this.Kind = kind;
	}
}

public struct AffixRange {
	public float minValue;
	public float maxValue;
	public BlockKey key;

	public AffixRange(ushort minValue, ushort maxValue, BlockKey key) {
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.key = key;
	}
}

public struct ScanStats {
	public int itemsScanned;
	public int blocksScanned;
	public int blocksSkipped;
	public int populatedBlocks;
	public int itemsLoaded;
};

public class Block {
	public readonly Block? parent;
	//AffixRange[BlockKey] ranges;
	public readonly SortedAffixRangeList ranges;
	public readonly BlockKind kind;
	public readonly Item[]? items;
	public readonly Block[]? blocks;

	public Block(BlockKind kind, Block? parent) {
		this.kind = kind;
		this.ranges = SortedAffixRangeList.Empty();
		this.parent = parent;
		if(this.kind == BlockKind.leaf) {
			this.items = new Item[IndexTest.itemsPerBlock];
		} else {
			this.blocks = new Block[IndexTest.blocksPerBlock];
		}
	}
}

public enum BlockKind : byte {
	leaf = 1,
	node = 2,
}

public record struct BlockKey(ulong ModHash, byte Kind);

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