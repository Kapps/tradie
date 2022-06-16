using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Storage;

[TestClass]
public class NodeListTests : TestBase {
	[TestMethod]
	public void TestLeafs() {
		var list = new NodeList(NodeKind.Leaf);
		
		Assert.AreEqual(NodeKind.Leaf, list.Kind);
		Assert.IsTrue(list.HasSpaceBeforeSplit);
		Assert.AreEqual(NodeList.ItemsPerBlock, list.Capacity);
		
		for(int i = 0; i < list.Capacity; i++) {
			Assert.AreEqual(i < list.Capacity - 1, list.HasSpaceBeforeSplit);
			list.Insert(0, new Item($"{list.Capacity - i - 1}", i, Array.Empty<Affix>()));
		}
		
		Assert.IsFalse(list.HasSpaceBeforeSplit);
		Assert.AreEqual(list.Capacity, list.Count);
		
		Assert.AreEqual(NodeList.ItemsPerBlock, list.Items.Length);
		Assert.AreEqual(0, list.Blocks.Length);

		list.Items.ToArray().Select(c => c.Id)
			.ShouldDeepEqual(Enumerable.Range(0, NodeList.ItemsPerBlock));
		
		var split = list.SplitLeft();
		Assert.AreEqual(NodeList.ItemsPerBlock / 2, split.Items.Length);
		Assert.AreEqual(NodeList.ItemsPerBlock / 2, list.Items.Length);
		
		var secondSplit = split.SplitRight();
		Assert.AreEqual(NodeList.ItemsPerBlock / 4, secondSplit.Items.Length);
		Assert.AreEqual(NodeList.ItemsPerBlock / 4, split.Items.Length);
		Assert.AreEqual(NodeList.ItemsPerBlock / 2, list.Items.Length);

		split.Items.ToArray().Select(c => c.Id)
			.ShouldDeepEqual(Enumerable.Range(0, NodeList.ItemsPerBlock).Take(split.Count));
		secondSplit.Items.ToArray().Select(c => c.Id)
			.ShouldDeepEqual(Enumerable.Range(0, NodeList.ItemsPerBlock).Skip(split.Count).Take(secondSplit.Count));
		list.Items.ToArray().Select(c => c.Id)
			.ShouldDeepEqual(Enumerable.Range(0, NodeList.ItemsPerBlock).Skip(split.Count + secondSplit.Count).Take(list.Count));
	}
	
	[TestMethod]
	public void TestNodes() {
		var tree = new ItemTree();
		var list = new NodeList(NodeKind.Block);
		
		Assert.AreEqual(NodeKind.Block, list.Kind);
		Assert.IsTrue(list.HasSpaceBeforeSplit);
		Assert.AreEqual(NodeList.BlocksPerBlock, list.Capacity);
		
		for(int i = 0; i < list.Capacity; i++) {
			Assert.AreEqual(i < list.Capacity - 1, list.HasSpaceBeforeSplit);
			list.Insert(0, new ItemTreeLeafNode(tree, null));
		}
		
		Assert.IsFalse(list.HasSpaceBeforeSplit);
		Assert.AreEqual(list.Capacity, list.Count);
		
		Assert.AreEqual(NodeList.BlocksPerBlock, list.Blocks.Length);
		Assert.AreEqual(0, list.Items.Length);

		var split = list.SplitLeft();
		Assert.AreEqual(NodeList.BlocksPerBlock / 2, split.Blocks.Length);
		Assert.AreEqual(NodeList.BlocksPerBlock / 2, list.Blocks.Length);
		
		var secondSplit = split.SplitRight();
		Assert.AreEqual(NodeList.BlocksPerBlock / 4, secondSplit.Blocks.Length);
		Assert.AreEqual(NodeList.BlocksPerBlock / 4, split.Blocks.Length);
		Assert.AreEqual(NodeList.BlocksPerBlock / 2, list.Blocks.Length);
	}

	[TestMethod]
	public void TestRemove() {
		var list = new NodeList(NodeKind.Leaf);

		for(int i = 0; i < list.Capacity; i++) {
			list.Insert(i, new Item($"item-{i}", i, Array.Empty<Affix>()));
		}
		
		Assert.AreEqual(list.Capacity, list.Count);

		var middle = list.RemoveAt<Item>(list.Capacity / 2);
		Assert.AreEqual(middle.Id, $"item-{list.Capacity / 2}");

		var first = list.RemoveAt<Item>(0);
		Assert.AreEqual("item-0", first.Id);
		
		var last = list.RemoveAt<Item>(list.Capacity - 2);
		Assert.AreEqual($"item-{list.Capacity - 1}", last.Id);
		
		Assert.AreEqual(list.Capacity - 3, list.Count);

		var receivedIds = list.Items.ToArray()
			.Select(c => c.ChaosEquivalentPrice)
			.ToArray();
		
		receivedIds.ShouldDeepEqual(
			Enumerable.Range(1, list.Capacity - 2)
				.Except(new[] { list.Capacity / 2 })
				.ToArray());
	}
}