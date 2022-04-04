using MessagePack;
using System.Diagnostics;
using System.Linq;

namespace Tradie.Indexer.Storage;

#if false
/// <summary>
/// Indicates whether a block is a leaf (contains items) or node (contains other blocks).
/// </summary>
public enum BlockKind : byte {
	Leaf = 1,
	Node = 2,
}

/// <summary>
/// A recursive multi-dimensional block range that can be used to efficiently look up optimized combinations of affixes.
/// </summary>
public class ItemTreeNode {
	/*
	 * Concept:
	 *
	 * OLD -- TO BE UPDATED
	 * Now we just keep things in the bottom left and create empty nodes so every node in the tree has the same depth.
	 * When increasing the depth, we just give the root BlocksPerBlock new children.
	 * All the previous children go underneath the first new block.
	 *
	 * Old Begins:
	 * 
	 * Start with a single node. This is the root node.
	 * The root node starts off with zero children.
	 * When a new item is inserted, the root node gets a child block created and the item placed in that block.
	 * When that block gets full, another block in the root is created.
	 *
	 * Once the root gets filled up, the first child block gets converted into a non-leaf.
	 * Then all the other previous leafs update and move underneath that new child.
	 * This frees up the root to have 3 empty child blocks that can store more nodes.
	 * Rinse and repeat, increasing depth as needed.
	 *
	 * All blocks are ordered by the price of the items inside them.
	 * This means it's important to do everything in order when iterating, as well as insert values in sorted order.
	 * This allows queries ordered by price (which is most queries) to only search until you find <count> matches.
	 * 
	 * So when inserting, we have to find the first block with a max price less than the item price.
	 * For example, with blocks:
	 * 
	 *                   Root
	 * 1 (1-3)    2 (5-6)    3 (6-8)    4 (12-14)
	 * 
	 * Then:
	 *	When inserting an item with value 0.5, it would go in 1.
	 *	When inserting an item with value 4, it would also go in 1.
	 *	When inserting an item with value 6, it would go in 2.
	 *
	 * In order to maintain efficient inserts though, we have to be able to append items without effectively
	 * copying all items that are a higher price than it over in memory.
	 * To accomplish this, when inserting into a block that is full, we split the block.
	 * A new block is created that contains the latter half of the items, while this block contains the first half.
	 * The new block is moved to the "next" block sequentially.
	 *
	 * For example, given the following blocks:
	 *
	 *              Root
	 * 1 [1, 1, 2, 3]	2 [3, 4, 5, 6]
	 *
	 * Then inserting an item with the value of 2, would result in:
	 *
	 *                    Root
	 * 1 [1, 1, 2]     2 [2, 3]    3 [3, 4, 5, 6]
	 *
	 * The node 1 got split into 1 and 2, and 3 moved over one spot in the parent.
	 * Then the new leaf with value 2 was inserted into block 1.
	 *
	 * In order to ensure the blocks remain balanced, before converting a node into a block node from a leaf,
	 * we must first check if any ancestor can store a new block.
	 * For example:
	 * 
	 *				                 Root
	 *       1		            2		           3		         4
	 * 5   |   |   |      6   |   |   |     7   |   |   |     8   |   |   |
	 * 9   |   |   |x4    10  |   |   |x4
	 * 
	 * When 9 becomes full, we don't convert it into a block node, even though 5 and 1 are also full.
	 * Instead, Root still has room available as 7 (plus siblings) and 8 (plus siblings) are currently leaf nodes.
	 * Therefore to keep the blocks balanced, 7 would become a new block node, and the old leafs moved under it.
	 * The result would look like:
	 * 
	 *				                 Root
	 *       1		             2		              3		             4
	 * 5   |   |   |       6   |   |   |        7   15   x    x      8   |   |   |
	 * 9   |   |   |x4     10  |   |   |x4     11   12   13   14
	 *
	 * Now node 3 has room for 3 new blocks, so the child of 9 would result in a cascade of nodes moving to make
	 * the previous 7 (now 11) and its siblings 12/13/14 be children under the new 7, and all nodes past 9
	 * to shift over one block until something is slotted into 15.
	 */
	
	/// <summary>
	/// Indicates how many items are present in a leaf block.
	/// </summary>
	private const int ItemsPerBlock = 16;
	/// <summary>
	/// Indicates how many nodes are present in a node block.
	/// </summary>
	private const int BlocksPerBlock = 10;

	/// <summary>
	/// The parent block that contains this one, or null if this block is a root.
	/// </summary>
	public ItemTreeNode? Parent { get; private set; }

	/// <summary>
	/// The (affix) range values contained within this block.
	/// </summary>
	public readonly SortedAffixRangeList Ranges;

	/// <summary>
	/// The type of this block (contains either items or more blocks).
	/// </summary>
	public BlockKind Kind { get; private set; }

	/// <summary>
	/// The items contained by this block, if it is a leaf; otherwise an empty span.
	/// </summary>
	public Span<Item> Items =>
		this.Kind == BlockKind.Leaf ? this._items.AsSpan(0, this._count) : Span<Item>.Empty;
	
	/// <summary>
	/// The child blocks contained by this block, unless it's a leaf; otherwise an empty span.
	/// </summary>
	public Span<ItemTreeNode> Blocks =>
		this.Kind == BlockKind.Node ? this._children.AsSpan(0, this._count) : Span<ItemTreeNode>.Empty;

	/// <summary>
	/// Indicates whether this block has room for more children (either items or blocks depending on Kind).
	/// </summary>
	public bool Full =>
		this.Kind == BlockKind.Leaf ? this._count == ItemsPerBlock : this._count == BlocksPerBlock;

	/// <summary>
	/// The minimum price of any item in this block.
	/// </summary>
	public float MinPrice { get; private set; }
	/// <summary>
	/// The maximum price of any item in this block.
	/// </summary>
	public float MaxPrice { get; private set; }

	/// <summary>
	/// Creates a new block of the given kind, with a parent (or null if this is the root block). 
	/// </summary>
	public ItemTreeNode(BlockKind kind, ItemTreeNode? parent) {
		this.Kind = kind;
		this.Ranges = SortedAffixRangeList.Empty();
		this.Parent = parent;
		this.MinPrice = 0;
		this.MaxPrice = float.MaxValue;
		
		if(this.Kind == BlockKind.Leaf)
			this._items = new Item[ItemsPerBlock];
		else
			this._children = new ItemTreeNode[BlocksPerBlock];
	}

	public void Insert(Item item) {
		if(this.Kind == BlockKind.Leaf) {
			this.InsertInLeaf(item);
			return;
		}

		var block = this.FindBlockForInsert(item);
		block.Insert(item);
	}

	private void InsertInLeaf(Item item) {
		Debug.Assert(this._count < ItemsPerBlock);
		Debug.Assert(this.Kind == BlockKind.Leaf);
			
		int insertIndex = this._items.AsSpan(0, this._count).BinarySearch(item);
		if(insertIndex < 0)
			insertIndex = ~insertIndex;
			
		for(int i = this._count; i > insertIndex; i--) {
			this._items![i] = this._items[i - 1];
		}

		this._items![insertIndex] = item;
		this._count++;
		UpdatePrices(item);
	}

	/// <summary>
	/// Finds the block for inserting the given item, creating one if need be.
	/// </summary>
	/// <remarks>
	/// Relies on this method being ordered in terms of price.
	/// If this block is being checked, it means the item price did not fit in any block before it.
	/// Given that prices are sorted in order from lowest to highest, this means find the first block with a MaxPrice
	/// value less than or equal to the item.
	/// </remarks>
	private ItemTreeNode FindBlockForInsert(Item item) {
		if(this._count == 0) {
			// When no blocks, it becomes the first block.
			return this.AppendBlockDirect();
		}
		
		for(int i = 0; i < this._count; i++) {
			var block = this._children![i];
			if(block.MaxPrice <= item.ChaosEquivalentPrice) {
				return block;
			}
		}

		return this._children![this._count - 1];
	}

	private ItemTreeNode AppendBlockDirect() {
		var child = this.CreateChild();
		this._children![this._count] = child;
		return child;
	}

	private ItemTreeNode CreateChild() {
		// Each block starts off as being a leaf block until it needs to be deepened.
		var child = new ItemTreeNode(BlockKind.Leaf, this);
		
		this._count++;
		this._freeBlocks--;
		if(this.Parent != null) {
			this.Parent._freeBlocks--;
		}

		return child;
	}

	private void InsertBlock(ItemTreeNode treeNode, int index) {
		ItemTreeNode? overflow = this._count < this._children!.Length ? null : this._children[this._count - 1];
		for(int i = this._count; i > index; i--) {
			this._children[i] = this._children[i - 1];
		}

		// If we have overflow, it becomes the first block after us in the parent.
		if(this.Parent == null) {
			// If we're the root though, then we have to increase the depth because it means we're the bottom right node.
			// The options would be increase at the node we were trying to insert at.
			// Or, keep the tree balanced and 
			throw new NotImplementedException();
		}

		int indexThis = Array.IndexOf(this.Parent._children!, this, 0, this.Parent._count);
		Debug.Assert(indexThis > 0);
		// If we're the last node of our parent, then we have to move to the first slot of the parent's parent > the parent.
		//            Root
		//        1               2 
		// 3    4    5    6   
		// In the above example, if we're 6, then we'd see no room is available in 1 as we're the last child.
		// So we have to look then at 1's position within Root.
		// As it's position 0, we'd insert at index 0+1 of Root, and then index from then on down.

		if(indexThis == BlocksPerBlock) {
			// If we can't 
		}
	}

	private void UpdatePrices(Item item) {
		for(var curr = this; curr != null; curr = curr.Parent) {
			if(curr.MaxPrice < item.ChaosEquivalentPrice) {
				curr.MaxPrice = item.ChaosEquivalentPrice;
				continue;
			}

			break;
		}

		for(var curr = this; curr != null; curr = curr.Parent) {
			if(curr.MinPrice > item.ChaosEquivalentPrice) {
				curr.MinPrice = item.ChaosEquivalentPrice;
				continue;
			}

			break;
		}
	}

	private void InsertBlock(ItemTreeNode treeNode) {
		throw new NotImplementedException();
	}

	private void AppendDepth() {
		throw new NotImplementedException();
	}

	private ItemTreeNode SplitChildren() {
		// Make a new child, move all children of this node under that child, update parents.
		throw new NotImplementedException();
	}

	private ItemTreeNode GetNextBlock() {
		// For a leaf block, parent block of index plus one. If parent full, then parent of parent, etc.
		// Make a new one if needed.
		throw new NotImplementedException();
	}
	
	private Item[]? _items; // Not null if leaf node.
	private ItemTreeNode[] _children; // Not null if non-leaf node.
	private ItemTreeNode? _leftSibling;
	private ItemTreeNode? _rightSibling;
	private int _count;
}
#endif