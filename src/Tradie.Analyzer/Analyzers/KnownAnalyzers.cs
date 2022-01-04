namespace Tradie.Analyzer.Analyzers;

/// <summary>
/// Registers hardcoded IDs for known analyzers, allowing efficient serialization by using small integers.
/// If an analyzer is no longer used, its ID must remain until all previous references are no longer in use.
/// </summary>
public enum KnownAnalyzers : ushort {
	ItemType = 1,
	Modifiers = 2
}