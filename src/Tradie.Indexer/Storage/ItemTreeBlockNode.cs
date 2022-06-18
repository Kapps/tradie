using System.Diagnostics;

namespace Tradie.Indexer.Storage;

public class ItemTreeBlockNode : ItemTreeNode {
	public ItemTreeBlockNode(ItemTree tree, NodeList? children)
		: base(NodeKind.Block, tree, children) {
		
	}

	/// <summary>
	/// Inserts the given node at the front of this list.
	/// </summary>
	public void InsertFront(ItemTreeNode node) {
		this.Insert(0, node);
	}
	
	/// <summary>
	/// Inserts the given node at the back of the list.
	/// </summary>
	public void InsertBack(ItemTreeNode node) {
		this.Insert(this.Children.Count, node);
	}

	/// <summary>
	/// Inserts the given node after an existing node.
	/// </summary>
	public void InsertAfter(ItemTreeNode after, ItemTreeNode node) {
		Debug.Assert(this.Children.Capacity > this.Children.Count);
		
		int index = this.FindIndexForPrice(after.MinPrice);
		this.Insert(index + 1, node);
	}

	protected internal override void VisitLeafs(Action<ItemTreeLeafNode> visitor) {
		foreach(var child in this.Children.Blocks) {
			child.VisitLeafs(visitor);
		}
	}

	private void Insert(int index, ItemTreeNode node) {
		Debug.Assert(this.Children.Count < this.Children.Capacity);
		this.Children.Insert(index, node);
		node.Parent = this;

		if(this.Children.Count == this.Children.Capacity) {
			// Once we get full, split this node.
			this.Split();
		}
		
		this.RecalculateDimensions();
		node.RecalculateDimensions();
	}

	private void Split() {
		var rightChildren = this.Children.SplitRight();
		var rightNode = new ItemTreeBlockNode(this.Tree, rightChildren);
		
		if(this.Parent == null) {
			// We're a root node, so create a new parent node and make that the root.
			var rootChildren = new NodeList(NodeKind.Block);
			rootChildren.Insert(0, this);
			rootChildren.Insert(1, rightNode);
			var parent = new ItemTreeBlockNode(this.Tree, rootChildren);
			parent.RecalculateDimensions();
		} else { 
			// Parent will deal with splitting if it's full.
			this.Parent.InsertAfter(this, rightNode);
		}
	}
	
	protected internal override ItemTreeLeafNode FindLeafForItem(in Item item) {
		var child = FindChildForPrice(item.ChaosEquivalentPrice);
		return child.FindLeafForItem(item);
	}

	private ItemTreeNode FindChildForPrice(float price) {
		int index = FindIndexForPrice(price);
		var blocks = this.Children.Blocks;
		if(index < 0) {
			// No block matched; we'll be expanding the last block.
			return blocks[^1];
		}

		return blocks[index];
	}
	
	private int FindIndexForPrice(float price) {
		var blocks = this.Children.Blocks;
		for(int i = 0; i < blocks.Length; i++) {
			// Find the minimum node that encompasses the price; sorted so no need to check min.

			var node = blocks[i];
			if(float.IsNaN(node.MaxPrice) || node.MaxPrice >= price) {
				return i;
			}
		}
		
		return ~blocks.Length;
	}
}