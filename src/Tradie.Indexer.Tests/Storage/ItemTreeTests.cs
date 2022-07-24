using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.Indexer.Search;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;
using Affix = Tradie.Indexer.Storage.Affix;

namespace Tradie.Indexer.Tests.Storage;

[TestClass]
public class ItemTreeTests : TestBase {

	[TestMethod]
	public void TestEmpty() {
		var tree = new ItemTree();
		Assert.IsNotNull(tree.Root);
		Assert.AreEqual(0, tree.Count);
	}

	[TestMethod]
	public void TestBulkInsertAndFind() {
		const int numItems = 1000;
		var tree = new ItemTree();
		var rng = new Random();
		
		var items = Enumerable.Range(1, numItems)
			.Select(c => new Item(c.ToString(), c, new Affix[] {
				new(new ModKey((ulong)c, ModKind.Explicit), c),
				new(new ModKey(numItems, ModKind.Implicit), (numItems - c) % (numItems / 5))
			}))
			.ToArray();
		int count = 1;
		//foreach(var item in items.OrderBy(c=>rng.Next())) {
		foreach(var item in items) {
			tree.Add(item);
			Assert.AreEqual(count, tree.Count);
			count++;
		}
		
		Assert.AreEqual(numItems, tree.Count);

		var searcher = new PriceSortedBlockSearcher();
		// Find items with different mods.
		foreach(var item in items) {
			var itemAffix = item.Affixes.First();
			var foundItem = searcher.Search(
				tree.Root,
				new SearchQuery(
					new[] {
						new SearchGroup(GroupKind.And,
							new[] {new SearchRange(itemAffix.Modifier, itemAffix.Value, itemAffix.Value)})
					},
					new SortOrder(SortKind.Price, null)
				),
				10
			).Single();
			Assert.AreEqual(foundItem, item);
		}

		// Find items with the same mod and overlapping values.
		foreach(var item in items) {
			var itemAffix = item.Affixes.Last();
			var itemBlocks = tree.Find(c => {
				var affix = c.Affixes.Get(itemAffix.Modifier.ModHash);
				if(affix.ModHash == default)
					return false;
				return affix.Implicit.Contains(itemAffix.Value);
				//ref var range = ref affix.GetRangeForModKind(itemAffix.Modifier.Kind);
				//return range.Contains(itemAffix.Value);
			}).ToArray();
			
			// Sorted by price with 200 apart, so each item should be in a different block.
			Console.WriteLine($"{item.Id} has {itemBlocks.Length} blocks.");
			//Assert.AreEqual(5, itemBlocks.Length);
			var found = itemBlocks
				.SelectMany(c => c.Children.ItemsSegment.Where(c => Math.Abs(c.Affixes.Last().Value - itemAffix.Value) < 0.1))
				.ToArray();
			Assert.AreEqual(5, found.Length);
		}
	}
}