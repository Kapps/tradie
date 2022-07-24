using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Indexer.Storage;

public sealed class HashedAffixRangeList : IDisposable {

	// TODO: Can we replace this with something closer to a bloom filter?
	// Just do a dictionary and accept collisions?
	// Would need to figure out number of actual distinct elements though...
	// but could base that off a formula based off depth or something like estimated count of a bloom filter?

	/// <summary>
	/// The number of elements this list is capable of storing before being resized.
	/// </summary>
	public int Capacity => this._affixRanges.Count;

	/// <summary>
	/// The number of elements currently present in this list.
	/// </summary>
	public int Count => this._affixRanges.Count;
	
    public static HashedAffixRangeList Empty() {
	    return new HashedAffixRangeList();
    }

    /// <summary>
    /// Returns the element at the zero-based index (which must be less than Count and greater than zero).
    /// </summary>
    public AffixRange this[int index] {
	    get {
		    if((uint)index >= this._affixRanges.Count)
			    throw new ArgumentOutOfRangeException(nameof(index));
		    return this._affixRanges.ElementAt(index).Value;
	    }
    }

    public void Add(AffixRange value) {
	    this._affixRanges.Add(value.ModHash, value);
    }

    public void Clear() {
	    this._affixRanges.Clear();
    }

    /*private ref AffixRange Insert(int index, AffixRange val) {
	    ref var entry = ref CollectionsMarshal.GetValueRefOrAddDefault(this._affixRanges, val.Key, out bool exists);
	    if(exists)
		    return ref entry;
	    return ref elements[index];
    }*/

    public ref AffixRange GetWithAddingDefault(ulong modHash, out bool exists) {
	    ref var res = ref CollectionsMarshal.GetValueRefOrAddDefault(this._affixRanges, modHash, out exists);
	    if(!exists)
		    res = new AffixRange(modHash);
	    return ref res;
    }

    public AffixRange Get(ulong modHash) {
	    if(this._affixRanges.TryGetValue(modHash, out var val))
		    return val;
	    return default;
    }
    
    /// <summary>
    /// Disposes of unmanaged memory used by this instance.
    /// </summary>
    public void Dispose() { 
    }

    public AffixRange[] ToArray() {
	    return this._affixRanges.Values.ToArray();
    }

    private Dictionary<ulong, AffixRange> _affixRanges = new();
}
