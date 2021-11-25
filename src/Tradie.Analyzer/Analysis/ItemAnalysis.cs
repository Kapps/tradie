using System.Collections;
using System.Collections.Concurrent;
using Tradie.Analyzer.Analysis;

namespace Tradie.Analyzer; 

/// <summary>
/// Provides data that was calculated as a result of analyzing a raw item.
/// All methods in this class are thread-safe.
/// </summary>
public class ItemAnalysis {
	private readonly ConcurrentDictionary<Guid, IAnalyzedProperties> _properties = new();

	/// <summary>
	/// Gets all of the properties added to this analysis.
	/// </summary>
	public IEnumerable<KeyValuePair<Guid, IAnalyzedProperties>> Properties => this._properties.ToArray();

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
		if(!this._properties.TryAdd(analyzerId, properties))
			throw new ArgumentException($"Analysis for analyzer {analyzerId} already exists.");
	}
}