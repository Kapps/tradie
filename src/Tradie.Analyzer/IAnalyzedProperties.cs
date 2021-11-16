namespace Tradie.Analyzer.Analyzers; 

/// <summary>
/// An extended set of properties for an item that are calculated from analyzing a raw item.
/// </summary>
public interface IAnalyzedProperties {
	/// <summary>
	/// The ID of the analyzer that provided these properties.
	/// </summary>
	public Guid Analyzer { get; }
}