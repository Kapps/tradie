using Tradie.Analyzer.Analyzers;

namespace Tradie.Indexer.Storage;

/// <summary>
/// A range of what values are present on a block for a given affix.
/// </summary>
public struct AffixRange {
	/// <summary>
	/// The lowest value found on an item in this range.
	/// </summary>
	public float MinValue;
	/// <summary>
	/// The highest value found on an item in this range.
	/// </summary>
	public float MaxValue;
	/// <summary>
	/// The location for the affix.
	/// </summary>
	public ModKey Key;

	public AffixRange(float minValue, float maxValue, ModKey key) {
		this.MinValue = minValue;
		this.MaxValue = maxValue;
		this.Key = key;
	}
}