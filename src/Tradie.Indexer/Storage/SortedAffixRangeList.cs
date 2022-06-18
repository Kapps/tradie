using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Indexer.Storage;

public sealed unsafe class SortedAffixRangeList : IDisposable {

	// TODO: Can we replace this with something closer to a bloom filter?
	// Just do a dictionary and accept collisions?
	// Would need to figure out number of actual distinct elements though...
	// but could base that off a formula based off depth or something like estimated count of a bloom filter?

	/// <summary>
	/// The number of elements this list is capable of storing before being resized.
	/// </summary>
	public int Capacity => this._capacity;
	/// <summary>
	/// The number of elements currently present in this list.
	/// </summary>
	public int Count => this._size;
	
    public static SortedAffixRangeList Empty() {
	    return new SortedAffixRangeList() {
		    _capacity = 0,
		    _size = 0,
		    _elementsPtr = IntPtr.Zero,
        };
    }

    /// <summary>
    /// Returns the element at the zero-based index (which must be less than Count and greater than zero).
    /// </summary>
    public AffixRange this[int index] {
	    get {
		    if((uint)index >= this._size)
			    throw new ArgumentOutOfRangeException(nameof(index));
		    return ((AffixRange*)(void*)this._elementsPtr)[index];
	    }
    }

    public void Add(AffixRange value) {
	    int index = this.GetIndex(value.Key);
	    if(index >= 0)
		    throw new DuplicateNameException();
	    this.Insert(~index, value);
    }

    public void Clear() {
	    this._size = 0;
    }

    private ref AffixRange Insert(int index, AffixRange val) {
	    if(this._size == this._capacity)
		    this.EnsureCapacity(this._size + 1);

	    AffixRange* elements = (AffixRange*)(void*)this._elementsPtr;
	    
	    if(index < this._size) {
		    Buffer.MemoryCopy(
			    elements + index,
			    elements + index + 1,
			    (this._capacity - index - 1) * sizeof(AffixRange),
			    (this._size - index) * sizeof(AffixRange)
			);
	    }

	    elements[index] = val;
	    ++this._size;
	    return ref elements[index];
    }

    private void EnsureCapacity(int min) {
	    int num = Math.Max((int)(this._capacity * 1.5), 4);
	    if(num < min)
		    num = min;

	    var next = Marshal.AllocHGlobal(sizeof(AffixRange) * num);
	    if(this._size > 0) {
		    Buffer.MemoryCopy(
			    (void*)this._elementsPtr, 
			    (void*)next, 
			    sizeof(AffixRange)* num, 
			    sizeof(AffixRange) * this._size
			);
	    }

	    if(this._elementsPtr != IntPtr.Zero) {
		    Marshal.FreeHGlobal(this._elementsPtr);
	    }

	    this._elementsPtr = next;
	    this._capacity = num;
    }

    public ref AffixRange GetWithAddingDefault(ModKey searchKey, out bool exists) {
	    var range = new AffixRange(0, 0, searchKey);
	    int index = this.GetIndex(searchKey);
	    if(index >= 0) {
		    exists = true;
		    return ref ((AffixRange*)(void*)this._elementsPtr)[index];
	    }

	    exists = false;
	    return ref this.Insert(~index, range);
    }

    public AffixRange Get(ModKey searchKey) {
	    int index = this.GetIndex(searchKey);
	    if(index >= 0) {
		    return *((AffixRange*)(void*)this._elementsPtr + index);
	    }

	    return default;
    }
    
    private void ReleaseUnmanagedResources() {
	    if(this._elementsPtr != IntPtr.Zero) {
		    Marshal.FreeHGlobal(this._elementsPtr);
		    this._elementsPtr = IntPtr.Zero;
	    }
    }

    /// <summary>
    /// Disposes of unmanaged memory used by this instance.
    /// </summary>
    public void Dispose() { 
	    this.ReleaseUnmanagedResources();
	    GC.SuppressFinalize(this);
    }

    public AffixRange[] ToArray() {
	    var results = new AffixRange[this._size];
	    var ptr = (AffixRange*)(void*)this._elementsPtr;
	    for(int i = 0; i < this._size; i++) {
		    results[i] = ptr[i];
	    }

	    return results;
    }

    ~SortedAffixRangeList() {
		this.ReleaseUnmanagedResources();
    }

    private int GetIndex(ModKey searchKey) {
	    if(this._elementsPtr == IntPtr.Zero) {
		    return -1;
	    }
	    
	    int lower = 0;
	    int upper = this._size - 1;

	    AffixRange* elements = (AffixRange*)(void*)this._elementsPtr;
	    while(lower <= upper) {
		    int mid = lower + (upper - lower) / 2;

		    var el = elements[mid];
		    if(el.Key == searchKey) {
			    return mid;
		    }
		    if(this.Compare(el.Key, searchKey) < 0) {
			    lower = mid + 1;
		    } else {
			    upper = mid - 1;
		    }
	    }

	    return ~lower;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private int Compare(ModKey a, ModKey b) {
	    int res = a.ModHash.CompareTo(b.ModHash);
	    if(res == 0) {
		    return a.Kind.CompareTo(b.Kind);
	    }

	    return res;
    }

    private IntPtr _elementsPtr;
    private int _capacity;
    private int _size;
}
