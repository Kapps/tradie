using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Storage;

[TestClass]
public class ItemTreeBlockNodeTests : TestBase {
	[TestMethod]
	public void TestBlockPricePropagation() {
		var tree = new ItemTree();
		var items = new NodeList(NodeKind.Block);
		items.Insert(0, new ItemTreeBlockNode(tree, null) { MinPrice = 3, MaxPrice = 6 });
		items.Insert(1, new ItemTreeBlockNode(tree, null) { MinPrice = 7, MaxPrice = 14 });

		var node = new ItemTreeBlockNode(tree, items);
		Assert.AreEqual(3, node.MinPrice);
		Assert.AreEqual(14, node.MaxPrice);
	}
	
	[TestMethod]
	public void TestParentPricePropagation() {
		var tree = new ItemTree();
		var items = new NodeList(NodeKind.Block);
		items.Insert(0, new ItemTreeBlockNode(tree, null) { MinPrice = 3, MaxPrice = 6 });
		items.Insert(1, new ItemTreeBlockNode(tree, null) { MinPrice = 7, MaxPrice = 14 });

		var node = new ItemTreeBlockNode(tree, items);
		Assert.AreEqual(3, node.MinPrice);
		Assert.AreEqual(14, node.MaxPrice);

		var child = new ItemTreeBlockNode(tree, null);
		node.InsertFront(child);
		
		Assert.IsTrue(float.IsNaN(child.MinPrice));
		Assert.IsTrue(float.IsNaN(child.MaxPrice));
	}

	[TestMethod]
	public void TestBlockSplit() {
		var tree = new ItemTree();
		var node = new ItemTreeBlockNode(tree, null);
		var leafs = Enumerable.Range(0, NodeList.BlocksPerBlock)
			.Select(c => new ItemTreeLeafNode(tree, null))
			.ToList();
		leafs.Take(NodeList.BlocksPerBlock - 1).ToList().ForEach(node.InsertBack);
		
		Assert.AreEqual(NodeList.BlocksPerBlock - 1, node.Children.Count);
		Assert.IsNull(node.Parent);
		
		node.InsertBack(leafs.Last());
		
		Assert.IsNotNull(node.Parent);

		var blocks = node.Parent.Children.Blocks;
		Assert.AreSame(blocks[0], node);
		Assert.AreNotSame(blocks[1], node);
		
		Assert.AreEqual(NodeList.BlocksPerBlock / 2, blocks[0].Children.Count);
		Assert.AreEqual(NodeList.BlocksPerBlock / 2, blocks[1].Children.Count);
		
		blocks[0].Children.Blocks.ToArray().ShouldDeepEqual(leafs.Take(NodeList.BlocksPerBlock / 2));
		blocks[1].Children.Blocks.ToArray().ShouldDeepEqual(leafs.Skip(NodeList.BlocksPerBlock / 2).Take(NodeList.BlocksPerBlock / 2));
	}
}