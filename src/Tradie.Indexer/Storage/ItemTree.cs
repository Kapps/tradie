using System.Collections;

namespace Tradie.Indexer.Storage;

public delegate bool BlockMatcher(ItemTreeNode block);

/// <summary>
/// A tree that stores multi-dimensional block ranges that can be used to efficiently look up optimized combinations of affixes.
/// At its core an ItemTree is very similar to a B+-Tree that searches in a different manner, using ordering for price sorting only.
/// </summary>
/// <remarks>
/// <para>
/// This is basically a B+ tree, at least for the storage and memory order of the nodes.
/// Effectively we need to recursively store blocks of items sorted by price.
/// Unlike a regular B+ tree though, we don't query by the sort key.
/// </para>
/// <para>
/// Instead, we also store the ranges of all affixes in the block.
/// In other words, for each affix present on any item, record the min and max value stored on any item in the block for that affix.
/// Effectively a block becomes a range of items, if the affixes on them were combined in optimal order.
/// These minima/maxima then propagate up to the parent block recursively, until reaching the root.
/// </para>
/// <para>
/// The goal then is <i>not</i> to use the tree to find a specific node (as the matched mods may be on different items),
/// but rather to filter out any blocks where we could not possibly find such a combination of mods to satisfy a search.
/// If a block is filtered out, then so too are all its child blocks, allowing us to skip large portions of the data set.
/// </para>
/// <para>
/// As most searches are sorted via price, and price is a property on all items, keeping items sorted by price
/// allows us to search left-to-right through children and automatically find results in a sorted fashion.
/// The result is we only need to find the first count items, and we'll know that those are the optimal ones.
/// </para>
/// </remarks>
public class ItemTree {
	/// <summary>
	/// The number of items present in the ItemTree.
	/// </summary>
	public int Count { get; private set; }

	/// <summary>
	/// The amount of levels deep this tree goes.
	/// An initial tree contains only a root node and starts at depth 1.
	/// </summary>
	public int Depth { get; private set; }

	/// <summary>
	/// Gets the current root node in the tree.
	/// </summary>
	public ItemTreeNode Root => this._root;

	/// <summary>
	/// Creates a new ItemTree with no items inside.
	/// </summary>
	public ItemTree() {
		this._root = new ItemTreeLeafNode(null);
		this.Depth = 1;
	}
	
	/// <summary>
	/// Inserts the given item into the tree.
	/// </summary>
	public void Add(Item item) {
		var node = this._root.FindLeafForItem(item);
		node.Add(item);
		if(this._root.Parent != null) {
			this._root = this._root.Parent;
			this.Depth++;
		}

		this.Count++;
	}

	/// <summary>
	/// Lazily finds and returns all nodes that match the given predicate.
	/// </summary>
	/// <param name="matcher"></param>
	/// <returns></returns>
	public IEnumerable<ItemTreeLeafNode> Find(BlockMatcher matcher) {
		return Find(_root, matcher);
	}

	private IEnumerable<ItemTreeLeafNode> Find(ItemTreeNode current, BlockMatcher matcher) {
		if(current.Kind == NodeKind.Leaf) {
			yield return (ItemTreeLeafNode)current;
		} else {
			foreach(var node in current.Children.BlocksSegment) {
				if(matcher(node)) {
					foreach(var sub in Find(node, matcher)) {
						yield return sub;
					}
				}
			}
		}
	}

	private ItemTreeNode _root;
}