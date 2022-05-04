using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
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
		var items = Enumerable.Range(1, numItems)
			.Select(c => new Item(c.ToString(), (float)c, new Affix[] {
				new(new ModKey((ulong)c, ModKind.Explicit), c),
				new(new ModKey(numItems, ModKind.Implicit), (numItems - c) % (numItems / 5))
			}))
			.ToArray();
		int count = 1;
		foreach(var item in items) {
			tree.Add(item);
			Assert.AreEqual(count, tree.Count);
			count++;
		}
		
		Assert.AreEqual(numItems, tree.Count);
		
		// Find items with different mods.
		foreach(var item in items) {
			var itemAffix = item.Affixes.First();
			var itemBlocks = tree.Find(c => {
				var affix = c.Affixes.Get(itemAffix.Modifier);
				if(affix.Key == default)
					return false;
				return affix.MinValue <= itemAffix.Value && affix.MaxValue >= itemAffix.Value;
			});
			Assert.IsTrue(itemBlocks.Single().Children.Items.ToArray().Contains(item));
		}
		
		// Find items with the same mod and overlapping values.
		foreach(var item in items) {
			var itemAffix = item.Affixes.Last();
			var itemBlocks = tree.Find(c => {
				var affix = c.Affixes.Get(itemAffix.Modifier);
				if(affix.Key == default)
					return false;
				return affix.MinValue <= itemAffix.Value && affix.MaxValue >= itemAffix.Value;
			}).ToArray();
			
			// Sorted by price with 200 apart, so each item should be in a different block.
			Assert.AreEqual(5, itemBlocks.Length);
			var found = itemBlocks
				.SelectMany(c => c.Children.ItemsSegment.Where(c => Math.Abs(c.Affixes.Last().Value - itemAffix.Value) < 0.1))
				.ToArray();
			Assert.AreEqual(5, found.Length);
		}
	}
}