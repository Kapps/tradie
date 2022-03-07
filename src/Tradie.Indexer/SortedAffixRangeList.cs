using System.Data;
using System.Runtime.InteropServices;

namespace Tradie.Indexer; 

public sealed unsafe class SortedAffixRangeList : IDisposable {
    public static SortedAffixRangeList Empty() {
	    return new SortedAffixRangeList() {
		    _capacity = 0,
		    _size = 0,
		    _elementsPtr = IntPtr.Zero,
        };
    }

    public void Add(AffixRange value) {
	    int index = getIndex(value.Key);
	    if(index >= 0)
		    throw new DuplicateNameException();
	    this.Insert(~index, value);
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
	    int index = getIndex(searchKey);
	    if(index >= 0) {
		    exists = true;
		    return ref ((AffixRange*)(void*)this._elementsPtr)[index];
	    }

	    exists = false;
	    return ref this.Insert(~index, range);

    }

    public AffixRange Get(ModKey searchKey) {
	    int index = getIndex(searchKey);
	    if(index > 0) {
		    return *(((AffixRange*)(void*)this._elementsPtr) + index);
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
	    ReleaseUnmanagedResources();
	    GC.SuppressFinalize(this);
    }

    ~SortedAffixRangeList() {
	    ReleaseUnmanagedResources();
    }

    private int getIndex(ModKey searchKey) {
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
		    if(Compare(el.Key, searchKey) < 0) {
			    lower = mid + 1;
		    } else {
			    upper = mid - 1;
		    }
	    }

	    return ~lower;
    }

    private int Compare(ModKey a, ModKey b) {
	    int res = a.ModHash.CompareTo(b.ModHash);
	    if(res == 0) {
		    return a.Location.CompareTo(b.Location);
	    }

	    return res;
    }

    private IntPtr _elementsPtr;
    private int _capacity;
    private int _size;
}

struct AffixRangeComparer : IComparer<AffixRange> {
	public int Compare(AffixRange x, AffixRange y)
	{
		if(x.Key.ModHash == y.Key.ModHash) {
			return x.Key.Location.CompareTo(y.Key.Location);
		}
		return x.Key.ModHash.CompareTo(y.Key.ModHash);
	}
}