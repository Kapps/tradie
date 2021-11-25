namespace Tradie.Analyzer.Analysis; 

/// <summary>
/// An extended set of properties for an item that are calculated from analyzing a raw item.
/// </summary>
public interface IAnalyzedProperties {
	/// <summary>
	/// Serializes these properties in binary format, in such a way that a reader can retrieve them.
	/// This may be just IDs, or may be other data required.
	/// </summary>
	void Serialize(BinaryWriter writer);
}