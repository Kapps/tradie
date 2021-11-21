using System.Collections;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Analyzer; 

/// <summary>
/// Provides data that was calculated as a result of analyzing a raw item.
/// </summary>
public class ItemAnalysis {
	private readonly Dictionary<Guid, IAnalyzedProperties> _properties = new();

	/// <summary>
	/// Gets all of the properties added to this analysis.
	/// </summary>
	public IEnumerable<IAnalyzedProperties> Properties => this._properties.Values;

	/// <summary>
	/// Returns the properties for this analyzer, or null if the analysis is not present.
	/// </summary>
	public IAnalyzedProperties? this[Guid analyzerId] {
		get {
			if(this._properties.TryGetValue(analyzerId, out var props))
				return props;
			return null;
		}
	}
	
	/// <summary>
	/// Appends an analyzed set of properties for this item to the properties collection.
	/// </summary>
	public void PushAnalysis<T>(Guid analyzerId, T properties) where T : IAnalyzedProperties {
		this._properties.Add(analyzerId, properties);
	}

	/// <summary>
	/// Serializes this item and its properties with the given writer.
	/// </summary>
	public void Serialize(BinaryWriter writer) {
		writer.Write(this._properties.Count);
		foreach(var prop in this._properties) {
			writer.Write(prop.Key.ToByteArray());
			prop.Value.Serialize(writer);
		}
	}
}