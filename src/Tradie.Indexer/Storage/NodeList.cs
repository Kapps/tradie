using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tradie.Indexer.Storage;

/// <summary>
/// Allows efficient storage of the elements for an ItemTreeNode, accounting for the storage type being
/// either an array of nodes or an array of items.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct NodeList {
	/// <summary>
	/// Indicates how many items are present in a leaf block.
	/// </summary>
	internal const int ItemsPerBlock = 32;
	/// <summary>
	/// Indicates how many nodes are present in a node block.
	/// </summary>
	internal const int BlocksPerBlock = 16;

	/// <summary>
	/// Gets the number of items actively stored in this list.
	/// </summary>
	public int Count => this._count;
	
	/// <summary>
	/// Gets the number of items that this list can store.
	/// </summary>
	public int Capacity => this._kind == NodeKind.Leaf ? ItemsPerBlock : BlocksPerBlock;

	/// <summary>
	/// Gets the number of items that this list can store prior to the container being split.
	/// </summary>
	public int CapacityBeforeSplit => Capacity - 1;
	
	/// <summary>
	/// The type of data stored in this collection; either items (leaf) or nodes (node).
	/// </summary>
	public NodeKind Kind => this._kind;
	
	/// <summary>
	/// The items contained by this block, if it is a leaf; otherwise an empty span.
	/// </summary>
	public Span<Item> Items =>
		this.Kind == NodeKind.Leaf ? this._items.AsSpan(0, this._count) : Span<Item>.Empty;
	
	internal ArraySegment<Item> ItemsSegment => new ArraySegment<Item>(this._items, 0, this._count);
	
	/// <summary>
	/// The child blocks contained by this block, unless it's a leaf; otherwise an empty span.
	/// </summary>
	public Span<ItemTreeNode> Blocks =>
		this.Kind == NodeKind.Block ? this._children.AsSpan(0, this._count) : Span<ItemTreeNode>.Empty;

	internal ArraySegment<ItemTreeNode> BlocksSegment => new ArraySegment<ItemTreeNode>(this._children, 0, this._count);

	/// <summary>
	/// Indicates whether this block has room for more children (either items or blocks depending on Kind) without splitting.
	/// </summary>
	public bool HasSpaceBeforeSplit =>
		this.Kind == NodeKind.Leaf ? this._count < ItemsPerBlock - 1 : this._count < BlocksPerBlock - 1;

	/// <summary>
	/// Creates a new NodeList storing items of the given type.
	/// </summary>
	public NodeList(NodeKind kind) {
		this._kind = kind;
		this._count = 0;
		this._padding = 0;
		
		if(this._kind == NodeKind.Leaf) {
			this._children = null; // Redundant -- we re-assign the same memory location right below but needed for errors.
			this._items = new Item[ItemsPerBlock];
		} else {
			this._items = null; // Redundant -- we re-assign the same memory location right below but needed for errors.
			this._children = new ItemTreeNode[BlocksPerBlock];
		}
	}

	/// <summary>
	/// Adds the given item to this collection at index, moving all items at and after it to the right as needed.
	/// It is invalid to invoke this method if this collection is not a leaf, or the collection is full.  
	/// </summary>
	public void Insert(int index, in Item item) {
		Debug.Assert(this._count < this._items!.Length);
		Debug.Assert(this.Kind == NodeKind.Leaf);
		
		for(int i = this._count; i > index; i--) {
			this._items[i] = this._items[i - 1];
		}

		this._items[index] = item;
		this._count++;
	}

	/// <summary>
	/// Adds the given node to this collection at index, moving all nodes at and after it to the right as needed.
	/// It is invalid to invoke this method if this collection is a leaf, or the collection is full.  
	/// </summary>
	public void Insert(int index, in ItemTreeNode item) {
		Debug.Assert(this._count < this._children!.Length);
		Debug.Assert(this.Kind == NodeKind.Block);
		
		for(int i = this._count; i > index; i--) {
			this._children[i] = this._children[i - 1];
		}

		this._children[index] = item;
		this._count++;
	}

	/// <summary>
	/// Splits a list down the middle, returning a new list that contains the second half of the elements of this list.
	/// The elements are moved to the new list, meaning they are removed from this one.
	/// </summary>
	public NodeList SplitRight() {
		var next = new NodeList(this._kind);
		int srcIndex = this._count / 2;
		ushort destCount = (ushort)((this._count + 1) / 2);
		if(this._kind == NodeKind.Leaf) {
			Array.Copy(this._items!, srcIndex, next._items!, 0, destCount);
		} else {
			Array.Copy(this._children!, srcIndex, next._children!, 0, destCount);
		}

		next._count = destCount;
		this._count -= destCount;
		return next;
	}
	
	/// <summary>
	/// Splits a list down the middle, returning a new list that contains the first half of the elements of this list.
	/// The elements are moved to the new list, meaning they are removed from this one.
	/// </summary>
	public NodeList SplitLeft() {
		var next = new NodeList(this._kind);
		ushort destCount = (ushort)(this._count / 2);
		if(this._kind == NodeKind.Leaf) {
			Array.Copy(this._items!, 0, next._items!, 0, destCount);
			Array.Copy(this._items!, destCount, this._items!, 0, this._count - destCount);
		} else {
			Array.Copy(this._children!, 0, next._children!, 0, destCount);
			Array.Copy(this._children!, destCount, this._children!, 0, this._count - destCount);
		}

		next._count = destCount;
		this._count -= destCount;
		return next;
	}

	/// <summary>
	/// Removes the entry at the given index, returning the removed entry.
	/// </summary>
	/// <typeparam name="T">Either Item, or ItemTreeNode depending on the list kind.</typeparam> 
	public T RemoveAt<T>(int index) {
		T res;
		if(this.Kind == NodeKind.Leaf) {
			Debug.Assert(typeof(T) == typeof(Item));
			res = Unsafe.As<T[]>(this._items!)[index];
			for(int i = index; i < this._count - 1; i++) {
				this._items![i] = this._items[i + 1];
			}
		} else {
			Debug.Assert(typeof(T) == typeof(ItemTreeNode));
			res = Unsafe.As<T[]>(this._children!)[index];
			for(int i = index; i < this._count - 1; i++) {
				this._children![i] = this._children[i + 1];
			}
		}

		this._count--;
		return res;
	}

	/*/// <summary>
	/// Returns the node in this list that best matches the given sorting key (chaos equivalent price).
	/// </summary>
	public ItemTreeNode FindNodeForPrice(int price) {
		Debug.Assert(this._kind == BlockKind.Node);
		
		foreach(var node in this._children!) {
			// Otherwise, find the minimum node that encompasses the price.
			if(node.MaxPrice >= price) {
				return node.FindLeafForItem(item);
			}
		}
	}*/
	
	// When would a left move be needed..?
	/*/// <summary>
	/// Adds the given item to this collection at index, moving all nodes at and before it to the left as needed.
	/// It is invalid to invoke this method if this collection is not a leaf, or the collection is full.  
	/// </summary>
	public void InsertAtWithLeftMove(in Item item, int index) {
		Debug.Assert(this._count < this._items!.Length);
		Debug.Assert(this.Kind == BlockKind.Leaf);
		
		for(int i = 0; i < index; i++) {
			this._items[i] = this._items[i + 1];
		}

		this._items[index] = item;
	}*/
	
	[FieldOffset(0)]
	//[MarshalAs(UnmanagedType.ByValArray, SizeConst = ItemsPerBlock)]
	private Item[]? _items;
	//[MarshalAs(UnmanagedType.ByValArray, SizeConst = BlocksPerBlock)]
	[FieldOffset(0)]
	private ItemTreeNode[]? _children;
	[FieldOffset(8)]
	private ushort _count;
	[FieldOffset(10)]
	private NodeKind _kind;
	[FieldOffset(11)]
	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
	private byte _padding;
}