using System.Diagnostics;

namespace Tradie.Indexer.Storage;

/// <summary>
/// A leaf node within an ItemTree. Leaf nodes contain the actual data elements (items) for the tree.
/// Leaf nodes may also be root nodes in the situation where the leaf is the one and only node in the tree.
/// </summary>
public sealed class ItemTreeLeafNode : ItemTreeNode {
	/// <summary>
	/// The node to the left of this one in the parent block node.
	/// </summary>
	public ItemTreeLeafNode? LeftSibling => this._leftSibling;
	/// <summary>
	/// The node to the right of this one in the parent block node.
	/// </summary>
	public ItemTreeLeafNode? RightSibling => this._rightSibling;
	
	public ItemTreeLeafNode(NodeList? children = null)
		: base(NodeKind.Leaf, children) {
		
	}
	
	protected internal override ItemTreeLeafNode FindLeafForItem(in Item item) {
		return this;
	}

	/// <summary>
	/// Inserts the given item in this node, splitting the node if required.
	/// </summary>
	public void Add(Item item) {
		Debug.Assert(this.Children.Count < NodeList.ItemsPerBlock);
		
		int index = this.FindIndexForItem(item);
		this.Insert(index, item);

		if(this.Children.Count < NodeList.ItemsPerBlock) {
			// We're not full, so nothing else to do.
			return;
		}
		
		// Otherwise, if we are full, let's try to offload something to a sibling.

		if(this._rightSibling?.Children is {HasSpaceBeforeSplit: true} && this._rightSibling.Parent == this.Parent) {
			// We don't have room, but our right sibling can take over our maximum item.
			throw new NotImplementedException("take max item not item");
			this._rightSibling.Insert(0, item);
			return;
		}

		if(this._leftSibling?.Children is {HasSpaceBeforeSplit: true} ls && this._leftSibling.Parent == this.Parent) {
			// We don't have room, but our left sibling can take over our smallest item.
			this._leftSibling.Insert(ls.Count, item);
			return;
		}
		
		// And if that doesn't work, we're going to have to split this node.
		// The resulting node gets added to this parent.

		this.Split();
	}

	private int FindIndexForItem(Item item) {
		int insertIndex = this.Children.Items.BinarySearch(item);
		if(insertIndex < 0)
			insertIndex = ~insertIndex;
		return insertIndex;
	}

	private void Insert(int index, Item item) {
		Debug.Assert(this.Children.Count < NodeList.ItemsPerBlock);
		this.Children.Insert(index, item);
		this.UpdatePrices(item);
	}

	private void Split() {
		var rightItems = this.Children.SplitRight();
		var rightNode = new ItemTreeLeafNode(rightItems);

		if(this._rightSibling != null) {
			this._rightSibling._leftSibling = rightNode;
			rightNode._rightSibling = this._rightSibling;
		}
		this._rightSibling = rightNode;
		
		if(this.Parent == null) {
			// We're a root node, so create a new parent node and make that the root.
			var rootChildren = new NodeList(NodeKind.Block);
			rootChildren.Insert(0, this);
			rootChildren.Insert(1, rightNode);
			var newRoot = new ItemTreeBlockNode(rootChildren);
		} else {
			this.Parent.InsertAfter(this, rightNode);
		}
		
		
		this.RecalculateDimensions();
		this._rightSibling.RecalculateDimensions();
	}

	
	private void UpdatePrices(Item item) {
		for(ItemTreeNode? curr = this; curr != null; curr = curr.Parent) {
			if(float.IsNaN(curr.MaxPrice) || curr.MaxPrice < item.ChaosEquivalentPrice) {
				curr.MaxPrice = item.ChaosEquivalentPrice;
				continue;
			}

			break;
		}

		for(ItemTreeNode? curr = this; curr != null; curr = curr.Parent) {
			if(float.IsNaN(curr.MinPrice) || curr.MinPrice > item.ChaosEquivalentPrice) {
				curr.MinPrice = item.ChaosEquivalentPrice;
				continue;
			}

			break;
		}
	}

	private ItemTreeLeafNode? _leftSibling;
	private ItemTreeLeafNode? _rightSibling;
}