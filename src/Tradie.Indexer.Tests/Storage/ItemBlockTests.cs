using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Storage;

#if false
[TestClass]
public class ItemBlockTests : TestBase {
	[TestMethod]
	public void TestInsertInLeaf() {
		var leaf = new ItemTreeNode(BlockKind.Leaf, null);
		var items = new[] { CreateItem(12), CreateItem(6), CreateItem(47), CreateItem(46.8f) };
		foreach(var item in items) {
			leaf.Insert(item);
		}

		var res = leaf.Items;
		res.ToArray().ShouldDeepEqual(new[] {
			items[1], items[0], items[3], items[2]
		});
	}

	[TestMethod]
	public void TestInsertingInEmptyRoot() {
		var root = new ItemTreeNode(BlockKind.Node, null);
		var item = CreateItem(12);
		Assert.AreEqual(0, root.Blocks.Length);
		
		root.Insert(item);

		Assert.AreEqual(1, root.Blocks.Length);
		Assert.AreEqual(BlockKind.Leaf, root.Blocks[0].Kind);
		root.Blocks[0].Items.ToArray().ShouldDeepEqual(new[] { item });
	}

	private Item CreateItem(float value) {
		return new Item(Guid.NewGuid().ToString(), value, Array.Empty<Affix>());
	}
}
#endif