using MessagePack;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Analyzer; 

/// <summary>
/// Provides data that was calculated as a result of analyzing a raw item.
/// All methods in this class are thread-safe.
/// </summary>
[DataContract, MessagePackObject]
public struct ItemAnalysis {
	//private readonly ConcurrentDictionary<ushort, IAnalyzedProperties> _properties = new();
	private readonly IAnalyzedProperties[] _properties;

	/// <summary>
	/// The ID of the item this analysis is for.
	/// </summary>
	[DataMember, Key(0)]
	public string ItemId { get; set; }

	/// <summary>
	/// Gets all of the properties added to this analysis.
	/// </summary>
	[DataMember, Key(1)]
	public IEnumerable<IAnalyzedProperties> Properties => this._properties.Where(c=>c != null);

	/// <summary>
	/// Returns the properties for this analyzer, or null if the analysis is not present.
	/// </summary>
	public IAnalyzedProperties? this[ushort analyzerId] => this._properties[IndexForAnalyzer(analyzerId)];

	/// <summary>
	/// Returns the properties for the given analyzer, typed as requested.
	/// If it is not present, a KeyNotFoundException is thrown.
	/// </summary>
	public T GetRequired<T>(ushort analyzerId) where T : IAnalyzedProperties {
		var res = this._properties[IndexForAnalyzer(analyzerId)];
		if(res == null) {
			throw new KeyNotFoundException();
		}

		return (T)res;
	}

	public ItemAnalysis(string itemId) {
		this.ItemId = itemId;
		//this._properties = new ConcurrentDictionary<ushort, IAnalyzedProperties>();
		this._properties = new IAnalyzedProperties[KnownAnalyzers.Max];
	}

	public ItemAnalysis(string itemId, IEnumerable<KeyValuePair<ushort, IAnalyzedProperties>> properties) : this(itemId) {
		foreach(var prop in properties) {
			this._properties[IndexForAnalyzer(prop.Key)] = prop.Value;
		}
	}

	public ItemAnalysis(string itemId, IEnumerable<IAnalyzedProperties> properties) : this(itemId) {
		foreach(var prop in properties) {
			this._properties[IndexForAnalyzer(KnownAnalyzers.GetAnalyzerIdForType(prop.GetType()))] = prop;
		}
	}
	
	/// <summary>
	/// Appends an analyzed set of properties for this item to the properties collection.
	/// </summary>
	public void PushAnalysis<T>(ushort analyzerId, T properties) where T : IAnalyzedProperties {
		int index = IndexForAnalyzer(analyzerId);
		if(this._properties[index] != null)
			throw new ArgumentException($"Analysis for analyzer {analyzerId} already exists.");
		this._properties[index] = properties;
	}

	private int IndexForAnalyzer(ushort analyzerId) => analyzerId - 1;

	/// <summary>
	/// Returns the properties of this item as a dictionary keyed by the analyzer kind.
	/// </summary>
	public Dictionary<ushort, IAnalyzedProperties> ToDictionary() {
		var res = new Dictionary<ushort, IAnalyzedProperties>(this._properties.Length);
		for(int i = 0; i < this._properties.Length; i++) {
			var prop = this._properties[i];
			if(prop == null)
				continue;
			res[(ushort)(i+1)] = prop;
		}

		return res;
	}
}