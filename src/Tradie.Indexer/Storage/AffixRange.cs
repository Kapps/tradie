using System.Runtime.InteropServices;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Indexer.Storage;

/// <summary>
/// A range of what values are present on a block for a given affix.
/// </summary>
public struct AffixRange {
	/// <summary>
	/// The size of the AffixRange struct, in bytes, including alignment.
	/// </summary>
	public static readonly int StructSize = Marshal.SizeOf<AffixRange>();

	/// <summary>
	/// The lowest total value found on an item in this range.
	/// </summary>
	public float MinValue;
	/// <summary>
	/// The highest total value found on an item in this range.
	/// </summary>
	public float MaxValue;
	/// <summary>
	/// The hash of the modifier for this affix.
	/// </summary>
	public ulong ModHash;
	
	public AffixRange(float minValue, float maxValue, ulong modHash) {
		this.MinValue = minValue;
		this.MaxValue = maxValue;
		this.ModHash = modHash;
	}
}