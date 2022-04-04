using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Storage;

[TestClass]
public class ItemTreeNodeTests : TestBase {
	[TestMethod]
	public void TestNodeSplit_WithRoomInParent() {
		var root = new ItemTreeBlockNode();
		var child = new ItemTreeLeafNode();
		root.InsertFront(child);
		for(int i = 0; i < NodeList.ItemsPerBlock - 1; i++) {
			child.Add(new Item($"{i}", i, Array.Empty<Affix>()));
		}
		
		Assert.AreSame(child.Parent, root);
		Assert.AreEqual(1, root.Children.Count);
		
		child.Add(new Item((NodeList.ItemsPerBlock - 1).ToString(), NodeList.ItemsPerBlock, Array.Empty<Affix>()));
		Assert.AreSame(child.Parent, root);
		Assert.AreEqual(2, root.Children.Count);
		Assert.AreSame(child, child.Parent!.Children.Blocks[0]);
		Assert.AreNotSame(child, child.Parent!.Children.Blocks[1]);
		
		child.Parent.Children.Blocks[0].Children.Items.ToArray().Select(c=>c.Id)
			.ShouldDeepEqual(Enumerable.Range(0, NodeList.ItemsPerBlock / 2).Select(c=>$"{c}"));
		
		child.Parent.Children.Blocks[1].Children.Items.ToArray().Select(c=>c.Id)
			.ShouldDeepEqual(Enumerable.Range(NodeList.ItemsPerBlock / 2, NodeList.ItemsPerBlock / 2).Select(c=>$"{c}"));
	}
}