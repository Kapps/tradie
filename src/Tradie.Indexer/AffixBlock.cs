using MessagePack;
using Tradie.Analyzer.Models;

namespace Tradie.Indexer;


/// <summary>
/// Indicates whether a block is a leaf (contains items) or node (contains other blocks).
/// </summary>
public enum BlockKind : byte {
	Leaf = 1,
	Node = 2,
}

/// <summary>
/// A key for finding a mod in a specific location.
/// </summary>
/// <param name="ModHash">The mod hash value for the affix.</param>
/// <param name="Location">The location of the modifier.</param>
public record struct ModKey(ulong ModHash, ModKind Location);

/// <summary>
/// A recursive multi-dimensional block range that can be used to efficiently look up a maximized combinations of affixes.
/// </summary>
[MessagePackObject]
public class AffixBlock {
	/// <summary>
	/// Indicates how many items are present in a leaf block.
	/// </summary>
	public const int ItemsPerBlock = 16;
	/// <summary>
	/// Indicates how many nodes are present in a node block.
	/// </summary>
	public const int BlocksPerBlock = 10;
	/// <summary>
	/// The maximum depth of the recursive block tree before storing items.
	/// </summary>
	public const int MaxDepth = 7;
	/// <summary>
	/// The maximum number of items a root AffixBlock is indirectly capable of storing.
	/// </summary>
	public static readonly int TotalCapacity = (int)Math.Pow(BlocksPerBlock, MaxDepth) * ItemsPerBlock;
	
	/// <summary>
	/// The parent block that contains this one, or null if this block is a root.
	/// </summary>
	public readonly AffixBlock? Parent;
	/// <summary>
	/// The (affix) range values contained within this block.
	/// </summary>
	public readonly SortedAffixRangeList Ranges;
	/// <summary>
	/// The type of this block (contains either items or more blocks).
	/// </summary>
	public readonly BlockKind Kind;
	/// <summary>
	/// The items contained by this block, if it is a leaf; otherwise null.
	/// </summary>
	public readonly Item[]? Items;
	/// <summary>
	/// The child blocks contained by this block, unless it's a leaf; otherwise null.
	/// </summary>
	public readonly AffixBlock[]? Blocks;

	/// <summary>
	/// Creates a new block of the given kind, with a parent (or null if this is the root block). 
	/// </summary>
	public AffixBlock(BlockKind kind, AffixBlock? parent) {
		this.Kind = kind;
		this.Ranges = SortedAffixRangeList.Empty();
		this.Parent = parent;
		if(this.Kind == BlockKind.Leaf) {
			this.Items = new Item[ItemsPerBlock];
		} else {
			this.Blocks = new AffixBlock[BlocksPerBlock];
		}
	}
}