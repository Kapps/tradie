using System.Collections;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Analyzer;

/// <summary>
/// Efficiently stores all possible analyzed properties from the pre-defined list.
/// </summary>
[JsonConverter(typeof(AnalyzedPropertyCollectionJsonConverter))]
public readonly struct AnalyzedPropertyCollection : IEnumerable<IAnalyzedProperties> {
	/// <summary>
	/// Returns all registered properties in this collection.
	/// </summary>
	public IEnumerable<IAnalyzedProperties> Properties => this._properties.Where(c => c != null)!;

	/// <summary>
	/// Returns the number of registered properties in this collection.
	/// </summary>
	public int Count => this._properties.Count(c => c != null);

	public AnalyzedPropertyCollection() { }

	public AnalyzedPropertyCollection(IDictionary<ushort, IAnalyzedProperties> properties) {
		foreach(var kvp in properties) {
			this.Add(kvp.Key, kvp.Value);
		}
	}

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
	
	/// <summary>
	/// Adds the analyzer and associates it with its id.
	/// If the analyzer was already added, an exception is thrown.
	/// </summary>
	public void Add(ushort analyzerId, IAnalyzedProperties properties) {
		int index = IndexForAnalyzer(analyzerId);
		if(this._properties[index] != null)
			throw new ArgumentException($"Analysis for analyzer {analyzerId} already exists.");
		this._properties[index] = properties;
	}

	/// <summary>
	/// Gets the analyzer with the given ID, or null if none was added for that id.
	/// </summary>
	public T? Get<T>(ushort analyzerId) where T : IAnalyzedProperties {
		var res = this._properties[this.IndexForAnalyzer(analyzerId)];
		if(res == null) {
			return default;
		}
		return (T?)res;
	}
	
	public IEnumerator<IAnalyzedProperties> GetEnumerator() {
		return new AnalyzedPropertyEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return new AnalyzedPropertyEnumerator(this);
	}

	private int IndexForAnalyzer(ushort analyzerId) => analyzerId - 1;

	private readonly IAnalyzedProperties?[] _properties = new IAnalyzedProperties[KnownAnalyzers.Max];
}

class AnalyzedPropertyEnumerator : IEnumerator<IAnalyzedProperties> {
	public AnalyzedPropertyEnumerator(AnalyzedPropertyCollection properties) {
		this._properties = properties;
	}
		
	public bool MoveNext() {
		while(this._currentAnalyzer < KnownAnalyzers.Max) {
			this._currentAnalyzer++;
			this._current = this._properties.Get<IAnalyzedProperties>(this._currentAnalyzer);
			if(this._current != null) {
				return true;
			}
		}

		return false;
	}

	public void Reset() {
		this._currentAnalyzer = 0;
	}

	IAnalyzedProperties IEnumerator<IAnalyzedProperties>.Current => this._current!;

	object IEnumerator.Current => this._current!;
		
	public void Dispose() {
	}

	private readonly AnalyzedPropertyCollection _properties;
	private IAnalyzedProperties? _current;
	private ushort _currentAnalyzer;
}