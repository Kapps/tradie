using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers; 

/// <summary>
/// Provides a way of storing analyzed properties about a raw item.
/// </summary>
public struct AnalyzedItem {
	/// <summary>
	/// The raw item being analyzed.
	/// </summary>
	public readonly Item RawItem;
}