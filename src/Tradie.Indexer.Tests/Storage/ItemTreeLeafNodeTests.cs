using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;
using Affix = Tradie.Indexer.Storage.Affix;

namespace Tradie.Indexer.Tests.Storage;

[TestClass]
public class ItemTreeLeafNodeTests : TestBase {
	[TestMethod]
	public void TestEmpty() {
		var tree = new ItemTree();
		var item = new Item("1", 12, Array.Empty<Affix>());
		var rootLeaf = new ItemTreeLeafNode(tree, null);
		
		Assert.AreEqual(rootLeaf, rootLeaf.FindLeafForItem(item));
		
		Assert.AreEqual(0, rootLeaf.Children.Count);
		Assert.AreEqual(0, rootLeaf.Affixes.Count);
		Assert.AreEqual(NodeKind.Leaf, rootLeaf.Kind);
		Assert.AreEqual(null, rootLeaf.Parent);
	}
	
	[TestMethod]
	public void TestPricePropagation() {
		var tree = new ItemTree();
		var items = new NodeList(NodeKind.Leaf);
		items.Insert(0, new Item("1", 1, Array.Empty<Affix>()));
		items.Insert(1, new Item("2", 6, Array.Empty<Affix>()));
		items.Insert(2, new Item("3", 12, Array.Empty<Affix>()));

		var node = new ItemTreeLeafNode(tree, items);
		Assert.AreEqual(1, node.MinPrice);
		Assert.AreEqual(12, node.MaxPrice);
	}
	
	[TestMethod]
	public void TestPriceUpdateOnInsert() {
		var tree = new ItemTree();
		var node = new ItemTreeLeafNode(tree, null);
		Assert.AreEqual(float.NaN, node.MinPrice);
		Assert.AreEqual(float.NaN, node.MaxPrice);
		
		node.Add(new Item("1", 1, Array.Empty<Affix>()));
		Assert.AreEqual(1, node.MinPrice);
		Assert.AreEqual(1, node.MaxPrice);
		
		node.Add(new Item("2", 12, Array.Empty<Affix>()));
		Assert.AreEqual(1, node.MinPrice);
		Assert.AreEqual(12, node.MaxPrice);
		
		node.Add(new Item("3", 6, Array.Empty<Affix>()));
		Assert.AreEqual(1, node.MinPrice);
		Assert.AreEqual(12, node.MaxPrice);
		
		node.Add(new Item("2", 18, Array.Empty<Affix>()));
		Assert.AreEqual(1, node.MinPrice);
		Assert.AreEqual(18, node.MaxPrice);
		
		node.Add(new Item("2", 0.5f, Array.Empty<Affix>()));
		Assert.AreEqual(0.5f, node.MinPrice);
		Assert.AreEqual(18, node.MaxPrice);
	}

	[TestMethod]
	[Ignore("Borrowing not yet implemented.")]	
	public void TestAdd_BorrowFromRightSibling() {
		var tree = new ItemTree();
		var root = new ItemTreeBlockNode(tree, null);
		var left = new ItemTreeLeafNode(tree, null);
		var right = new ItemTreeLeafNode(tree, null);
		
		root.InsertFront(left);
		root.InsertBack(right);

		for(int i = 0; i < NodeList.ItemsPerBlock - 1; i++) {
			left.Add(new Item($"{i}", i, Array.Empty<Affix>()));
		}

		Assert.IsFalse(left.Children.HasSpaceBeforeSplit);
		Assert.IsTrue(right.Children.HasSpaceBeforeSplit);
		Assert.AreEqual(0, right.Children.Count);
		
		// Inserting 15 should insert, and then move 15 to the right.
		left.Add(new Item($"{NodeList.ItemsPerBlock-1}", NodeList.ItemsPerBlock-1, Array.Empty<Affix>()));
		
		Assert.IsFalse(left.Children.HasSpaceBeforeSplit);
		Assert.IsTrue(right.Children.HasSpaceBeforeSplit);
		
		Assert.AreEqual(1, right.Children.Count);
		Assert.AreEqual(NodeList.ItemsPerBlock-1, left.Children.Count);
		Assert.AreEqual($"{NodeList.ItemsPerBlock-1}", right.Children.Items[0].Id);
		Assert.AreEqual($"{NodeList.ItemsPerBlock-2}", left.Children.Items[^1]);
	}
	
	[TestMethod]
	public void TestNodeSplit_AsRoot() {
		var tree = new ItemTree();
		var originalNode = new ItemTreeLeafNode(tree, null);
		for(int i = 0; i < NodeList.ItemsPerBlock - 1; i++) {
			originalNode.Add(new Item($"{i}", i, Array.Empty<Affix>()));
		}
		
		Assert.IsFalse(originalNode.Children.HasSpaceBeforeSplit);
		Assert.IsNull(originalNode.Parent);
		
		originalNode.Add(new Item((NodeList.ItemsPerBlock - 1).ToString(), NodeList.ItemsPerBlock, Array.Empty<Affix>()));
		
		Assert.IsNotNull(originalNode.Parent);
		Assert.AreEqual(2, originalNode.Parent.Children.Count);

		var leafBlocks = originalNode.Parent.Children.Blocks;
		Assert.AreSame(leafBlocks[0], originalNode);
		Assert.AreNotSame(leafBlocks[1], originalNode);
		
		Assert.AreEqual(NodeList.ItemsPerBlock / 2, leafBlocks[0].Children.Count);
		Assert.AreEqual(NodeList.ItemsPerBlock / 2, leafBlocks[1].Children.Count);

		leafBlocks[0].Children.Items.ToArray().Select(c=>c.Id)
			.ShouldDeepEqual(Enumerable.Range(0, NodeList.ItemsPerBlock / 2).Select(c=>$"{c}"));
		
		leafBlocks[1].Children.Items.ToArray().Select(c=>c.Id)
			.ShouldDeepEqual(Enumerable.Range(NodeList.ItemsPerBlock / 2, NodeList.ItemsPerBlock / 2).Select(c=>$"{c}"));
	}

	[TestMethod]
	public void TestAffixCalculation() {
		var tree = new ItemTree();
		var node = new ItemTreeLeafNode(tree, null);
		node.Add(new Item("a", 1, new[] {
			new Affix(new ModKey(12, ModKind.Explicit), 12),
			new Affix(new ModKey(16, ModKind.Explicit), 16)
		}));
		node.Add(new Item("b", 2, new[] {
			new Affix(new ModKey(12, ModKind.Explicit), 32),
			new Affix(new ModKey(16, ModKind.Implicit), 100)
		}));

		var affixes = node.Affixes.ToArray().OrderBy(c=>c.Key.ModHash).ThenBy(c=>c.Key.Kind).ToArray();
		Console.WriteLine(affixes);
		affixes.ShouldDeepEqual(new AffixRange[] {
			new(12, 32, new ModKey(12, ModKind.Explicit)),
			new(100, 100, new ModKey(16, ModKind.Implicit)),
			new(16, 16, new ModKey(16, ModKind.Explicit))
		});
	}
}