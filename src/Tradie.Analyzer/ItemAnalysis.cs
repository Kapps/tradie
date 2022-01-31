using MessagePack;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Tradie.Analyzer; 

/// <summary>
/// Provides data that was calculated as a result of analyzing a raw item.
/// All methods in this class are thread-safe.
/// </summary>
[DataContract, MessagePackObject]
public readonly struct ItemAnalysis {
	private readonly ConcurrentDictionary<ushort, IAnalyzedProperties> _properties = new();

	/// <summary>
	/// The ID of the item this analysis is for.
	/// </summary>
	[DataMember, Key(0)]
	public readonly string ItemId;
	
	/// <summary>
	/// Gets all of the properties added to this analysis.
	/// </summary>
	[DataMember, Key(1)]
	public ICollection<KeyValuePair<ushort, IAnalyzedProperties>> Properties => this._properties.ToArray();

	/// <summary>
	/// Returns the properties for this analyzer, or null if the analysis is not present.
	/// </summary>
	public IAnalyzedProperties? this[ushort analyzerId] {
		get {
			if(this._properties.TryGetValue(analyzerId, out var props))
				return props;
			return null;
		}
	}

	public ItemAnalysis(string itemId) {
		this.ItemId = itemId;
		this._properties = new ConcurrentDictionary<ushort, IAnalyzedProperties>();
	}

	public ItemAnalysis(string itemId, ICollection<KeyValuePair<ushort, IAnalyzedProperties>> properties) {
		this.ItemId = itemId;
		this._properties = new(properties);
	}
	
	/// <summary>
	/// Appends an analyzed set of properties for this item to the properties collection.
	/// </summary>
	public void PushAnalysis<T>(ushort analyzerId, T properties) where T : IAnalyzedProperties {
		if(!this._properties.TryAdd(analyzerId, properties))
			throw new ArgumentException($"Analysis for analyzer {analyzerId} already exists.");
	}
}