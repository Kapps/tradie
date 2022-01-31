using System.Diagnostics;
using System.Text.Json;

namespace Tradie.Indexer; 

public class IndexTest {
	public const int itemsPerBlock = 64;
	public const int blocksPerBlock = 32;
	public const int maxDepth = 5;
	public const int itemsToAllocate = 50_000_000;

	
	static Random rng = new Random();
	

	int itemsAllocated;


	byte searchKind = 1;

	private ushort[] searchMods = Array.Empty<ushort>();
	//HashSet<ushort> searchMods = new HashSet<ushort>();
	Item maxItem;
	int maxSum;

	byte numQueries = 2;
	ushort numMods = 6000;
	int numTimesToScan = 100;

	ScanStats stats;

	uint getBlockKey(ushort modifier, byte kind) {
		return (uint)(modifier << 16) | kind;
	}

	Item createItem() {
		var affixCount = rng.Next(4, 7);
		var id = Guid.NewGuid();
		var affixes = new Affix[affixCount];
		
		for(int i = 0; i < affixCount; i++) {
			var kind = (byte)rng.Next(1, 4);
			do {
				var modifier = (ushort)rng.Next(1, numMods);
				bool next = false;
				for(int j = 0; j < i; j++) {
					if(affixes[j].modifier == modifier) {
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

		affixes = affixes.OrderBy(c => c.modifier).ToArray();
		return new Item(id, affixes);
	}
	
	public void searchByBruteForce() {
		var sw = Stopwatch.StartNew();
		var items = new Item[itemsToAllocate];

		for(int i = 0; i < items.Length; i++) {
			items[i] = this.createItem();
		}
		
		Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds to generate {itemsToAllocate} items.");
		
		sw.Restart();


		foreach(var item in items) {
			stats.itemsScanned++;
			int sum = 0;
			foreach(var affix in item.affixes) {
				if(this.searchMods.Contains(affix.modifier)) {
					sum += affix.value;
				}
			}
			
			if (sum > maxSum) {
				maxSum = sum;
				maxItem = item;
			}
		}

		Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds to search items.");
	}
	
	public void resetStats() {
		stats.blocksScanned = 0;
		stats.blocksSkipped = 0;
		stats.itemsScanned = 0;
	}
	
	public void printResults() {
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
		", stats.blocksScanned, stats.blocksSkipped, stats.populatedBlocks, stats.itemsScanned, stats.itemsScanned / itemsPerBlock, itemsToAllocate - stats.itemsScanned);
	}
	
	public void searchByBlocks() {
		var sw = Stopwatch.StartNew();

	    Block root = new Block(BlockKind.node, null);
	    
	    populateBlock(ref root, null, 0);

	    Console.WriteLine("Took {0} milliseconds to generate {1} items.", sw.ElapsedMilliseconds, itemsAllocated);

	    //auto rootSum = root.ranges.get(getBlockKey(searchMod1, searchKind), AffixRange.init).maxValue 
	    //    + root.ranges.get(getBlockKey(searchMod2, searchKind), AffixRange.init).maxValue;

	    int totalMS = 0;
	    for(int i = 0; i < numTimesToScan; i++) {
	        maxItem = default;
	        maxSum = 0;
	        
	        prepareSearchMods();
	        
	        sw.Restart();
	        searchInBlock(root);
	        Console.WriteLine("Took {0} milliseconds to search for items for mods {1}.", sw.ElapsedMilliseconds, searchMods);
	        totalMS += (int)sw.ElapsedMilliseconds;
	        printResults();
	        var rootSum = getSum(root);
	        Console.WriteLine("Max possible sum is {0}, and received sum is {1}", rootSum, maxSum);
	        
	        maxItem = default;
	        resetStats();
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
	        foreach(var item in block.items!) {
	            stats.itemsScanned++;
	            int sum = 0;
	            foreach(var affix in item.affixes) {
		            if(this.searchMods.Contains(affix.modifier)) {
			            sum += affix.value;
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

	void populateBlock(ref Block block, Block? parent, int depth) {
	    if(itemsAllocated > itemsToAllocate) {
	        return;
		}
	    if(depth == maxDepth) {
		    block = new Block(BlockKind.leaf, parent);

		    for(int i = 0; i < block.items!.Length; i++) {
			    block.items[i] = createItem();

			    foreach(var affix in block.items[i].affixes) {
				    updateBlocks(block, affix);
			    }
		    }
	        
	        stats.populatedBlocks++; 

	        itemsAllocated += itemsPerBlock;
	        if((itemsAllocated % 100000) == 0) {
	            Console.WriteLine("Finished allocating {0} items", itemsAllocated);
			}
		} else {
		    block = new Block(BlockKind.node, parent);

		    for(int i = 0; i < block.blocks!.Length; i++) {
			    this.populateBlock(ref block.blocks![i], block, depth + 1);
		    }
	    }
	}

	/*void updateBlocks(Block? block, Affix affix) {
		if(block == null) return;

		var key = getBlockKey(affix.modifier, affix.kind); 
		ref var range = ref block.ranges.GetWithAddingDefault(key, out var exists);
		if(!exists) {
			range.minValue = range.maxValue = affix.value;
			range.key = key;
	        
	        updateBlocks(block.parent, affix);
	    } else {
			if(affix.value < range.minValue) {
	            range.minValue  = affix.value;
	            updateBlocks(block.parent, affix);
			}
	        if(affix.value > range.maxValue) {
	            range.maxValue = affix.value;
	            updateBlocks(block.parent, affix);
			}
		}
	}*/
	
	void updateBlocks(Block? block, Affix affix) {
		if(block == null) return;

		var key = getBlockKey(affix.modifier, affix.kind); 
		ref var range = ref block.ranges.GetWithAddingDefault(key, out var exists);
		if(!exists) {
			range.minValue = range.maxValue = affix.value;
			range.key = key;
	        
	        updateBlocks(block.parent, affix);
	    } else {
			if(affix.value < range.minValue) {
	            range.minValue  = affix.value;
	            updateBlocks(block.parent, affix);
			}
	        if(affix.value > range.maxValue) {
	            range.maxValue = affix.value;
	            updateBlocks(block.parent, affix);
			}
		}
	}
	
	void prepareSearchMods() {
		//searchMods.Clear();
		this.searchMods = Array.Empty<ushort>();
		//searchMods = [];
		this.searchMods = Enumerable.Range(0, this.numQueries).Select(_ => (ushort)rng.Next(1, this.numMods + 1))
			.ToArray();
	}
}

public readonly struct Item {
	public readonly Guid id;
	public readonly Affix[] affixes;

	public Item(Guid id, Affix[] affixes) {
		this.id = id;
		this.affixes = affixes;
	}
}

public readonly struct Affix {
	public readonly ushort modifier;
	public readonly ushort value;
	public readonly byte kind;

	public Affix(ushort modifier, ushort value, byte kind) {
		this.modifier = modifier;
		this.value = value;
		this.kind = kind;
	}
}

public struct AffixRange {
	public ushort minValue;
	public ushort maxValue;
	public uint key;

	public AffixRange(ushort minValue, ushort maxValue, uint key) {
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