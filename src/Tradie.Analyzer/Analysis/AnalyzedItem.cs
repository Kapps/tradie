using Tradie.Common.RawModels;

namespace Tradie.Analyzer; 

/// <summary>
/// Provides a way of storing analyzed properties about a raw item.
/// </summary>
public struct AnalyzedItem {
	/// <summary>
	/// The raw item being analyzed.
	/// </summary>
	public readonly Item RawItem;
	/// <summary>
	/// The analysis done on this item.
	/// </summary>
	public readonly ItemAnalysis Analysis;

	/// <summary>
	/// Creates a new AnalyzedItem from the given raw item.
	/// </summary>
	public AnalyzedItem(Item rawItem) {
		this.RawItem = rawItem;
		this.Analysis = new();
	}
}