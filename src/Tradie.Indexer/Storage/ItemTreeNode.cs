using MessagePack;
using System.Diagnostics;
using System.Linq;

namespace Tradie.Indexer.Storage;

/// <summary>
/// Indicates whether a block is a leaf (contains items) or node (contains other blocks).
/// </summary>
public enum NodeKind : byte {
	Leaf = 1,
	Block = 2,
}

/// <summary>
/// A recursive multi-dimensional block range that can be used to efficiently look up optimized combinations of affixes.
/// </summary>
public abstract class ItemTreeNode {
	/// <summary>
	/// The parent block that contains this one, or null if this block is a root.
	/// </summary>
	public ItemTreeBlockNode? Parent { get; protected internal set; }

	/// <summary>
	/// The affix range values contained within this block.
	/// This reference should never be modified.
	/// </summary>
	public ref SortedAffixRangeList Affixes => ref this._affixes;

	/*/// <summary>
	/// Returns a copy of the affix ranges present in this node.
	/// </summary>
	public IEnumerable<AffixRange> Affixes => this._affixes.GetAffixes();*/

	/// <summary>
	/// Returns a reference to the children of this node.
	/// </summary>
	public ref NodeList Children => ref this._children;

	/// <summary>
	/// The type of this block (contains either items or more blocks).
	/// </summary>
	public NodeKind Kind { get; private set; }

	/// <summary>
	/// The minimum price of any item in this block.
	/// </summary>
	public float MinPrice { get; internal set; }
	/// <summary>
	/// The maximum price of any item in this block.
	/// </summary>
	public float MaxPrice { get; internal set; }
	
	/// <summary>
	/// Creates a new block of the given kind, with a parent (or null if this is the root block). 
	/// </summary>
	protected ItemTreeNode(NodeKind kind, NodeList? children = null) {
		this.Kind = kind;
		this.Affixes = SortedAffixRangeList.Empty();
		this._children = children ?? new NodeList(kind);
		this._affixes = new();

		if(this.Kind == NodeKind.Block) {
			foreach(var child in this._children.Blocks) {
				child.Parent = (ItemTreeBlockNode)this;
			}
		}

		this.RecalculateDimensions();
	}

	protected internal void RecalculateDimensions() {
		this.RecalculatePrices();
		this.RecalculateAffixes();
	}

	protected void RecalculateAffixes() {
		this._affixes.Clear();
		if(this._children.Count == 0) {
			return;
		}

		if(this.Kind == NodeKind.Leaf) {
			foreach(ref var item in this._children.Items) {
				foreach(var affix in item.Affixes) {
					UpdateAffixIfNeeded(affix);
				}
			}
		} else {
			foreach(var block in this._children.Blocks) {
				var affixes = block._affixes;
				for(int i = 0; i < affixes.Count; i++) {
					var affix = affixes[i];
					ref var ours = ref this._affixes.GetWithAddingDefault(affix.Key, out _);
					if(affix.MinValue < ours.MinValue) {
						UpdateAffixIfNeeded(new Affix(affix.Key, affix.MinValue));
					}

					if(affix.MaxValue > ours.MaxValue) {
						UpdateAffixIfNeeded(new Affix(affix.Key, affix.MaxValue));
					}
				}
			}
		}
	}

	protected void UpdateAffixIfNeeded(Affix affix) {
		ref var range = ref this._affixes.GetWithAddingDefault(affix.Modifier, out var exists);
		if(!exists || range.MinValue > affix.Value) {
			range.MinValue = affix.Value;
			if(Parent != null) {
				Parent.UpdateAffixIfNeeded(affix);
			}
		}

		if(!exists || range.MaxValue < affix.Value) {
			range.MaxValue = affix.Value;
			if(Parent != null) {
				Parent.UpdateAffixIfNeeded(affix);
			}
		}
	}


	private void RecalculatePrices() {
		if(this._children.Count == 0) {
			this.MinPrice = float.NaN;
			this.MaxPrice = float.NaN;
			return;
		}
		
		float oldMin = this.MinPrice, oldMax = this.MaxPrice;
		if(this.Kind == NodeKind.Leaf) {
			var items = this._children.Items;
			this.MinPrice = items[0].ChaosEquivalentPrice;
			this.MaxPrice = items[^1].ChaosEquivalentPrice;
		} else {
			var blocks = this._children.Blocks;
			this.MinPrice = blocks[0].MinPrice;
			this.MaxPrice = blocks[^1].MaxPrice;
		}

		if(Math.Abs(this.MinPrice - oldMin) >= 0.01) {
			for(var parent = this.Parent; parent != null; parent = parent.Parent) {
				if(parent.MinPrice <= this.MinPrice)
					break;
				parent.MinPrice = this.MinPrice;
			}
		}
		
		if(Math.Abs(this.MaxPrice - oldMax) >= 0.01) {
			for(var parent = this.Parent; parent != null; parent = parent.Parent) {
				if(parent.MaxPrice >= this.MaxPrice)
					break;
				parent.MaxPrice = this.MaxPrice;
			}
		}
	}

	/// <summary>
	/// Returns the leaf node that should be used as a first attempt to store the given item.
	/// As splitting may occur when an item is inserted, this is _not_ necessarily
	/// the node that will ultimately contain the item.
	/// </summary>
	protected internal abstract ItemTreeLeafNode FindLeafForItem(in Item item);

	// protected internal abstract ItemTreeLeafNode Add(Item item);

	private NodeList _children;
	private SortedAffixRangeList _affixes;
}
