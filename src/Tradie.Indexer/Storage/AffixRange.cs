using System.Runtime.InteropServices;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;

namespace Tradie.Indexer.Storage;

/// <summary>
/// A range of what values are present on a block for a given affix.
/// </summary>
public struct AffixRange {
	/*/// <summary>
	/// The size of the AffixRange struct, in bytes, including alignment.
	/// </summary>
	public static readonly int StructSize = Marshal.SizeOf<AffixRange>();*/
	
	/// <summary>
	/// The hash of the modifier for this affix.
	/// </summary>
	public readonly ulong ModHash;

	/// <summary>
	/// The range of values for the implicit affixes in this range.
	/// </summary>
	public FloatRange Implicit;
	/// <summary>
	/// The range of values for the explicit affixes in this range.
	/// </summary>
	public FloatRange Explicit;
	/// <summary>
	/// The range of values for the summed suffixes in this range.
	/// </summary>
	public FloatRange Total;

	/// <summary>
	/// Returns a reference to the FloatRange for this type of modifier.
	/// This reference _must not_ be kept beyond the scope of the struct.
	/// </summary>
	public unsafe ref FloatRange GetRangeForModKind(ModKind kind) {
		switch(kind.GetCategory()) {
			case ModKindCategory.Enchant:
			case ModKindCategory.Implicit:
				fixed(FloatRange* ptr = &this.Implicit) {
					return ref *ptr;
				}
			case ModKindCategory.Explicit:
				fixed(FloatRange* ptr = &this.Explicit) {
					return ref *ptr;
				}
			case ModKindCategory.Pseudo:
				fixed(FloatRange* ptr = &this.Total) {
					return ref *ptr;
				}
			default:
				throw new ArgumentException("Unrecognized ModKindCategory: " + kind.GetCategory());		
		}
	}

	public AffixRange(ulong modHash) {
		this.ModHash = modHash;
		this.Implicit = this.Explicit = this.Total = FloatRange.NaN;
	}
}

/// <summary>
/// A range of floating point values between a min and max value (inclusive for both).
/// </summary>
public struct FloatRange {
	/// <summary>
	/// Returns a FloatRange with the min and max values both set to NaN.
	/// </summary>
	public static FloatRange NaN => new(float.NaN, float.NaN);
	
	/// <summary>
	/// The highest total value found on an item in this range.
	/// </summary>
	public float MinValue;
	/// <summary>
	/// The lowest total value found on an item in this range.
	/// </summary>
	public float MaxValue;
	
	public FloatRange(float minValue, float maxValue) {
		this.MinValue = minValue;
		this.MaxValue = maxValue;
	}

	/// <summary>
	/// Updates the MinValue and MaxValue fields of this instance to include the values from the given item.
	/// Returns true if either the MinValue or MaxValue of this instance were changed.
	/// </summary>
	public bool Expand(FloatRange other) {
		bool res = false;
		if (float.IsNaN(this.MinValue) || other.MinValue < this.MinValue) {
			this.MinValue = other.MinValue;
			res = !float.IsNaN(other.MinValue);
		}
		if (float.IsNaN(this.MaxValue) || other.MaxValue > this.MaxValue) {
			this.MaxValue = other.MaxValue;
			res = !float.IsNaN(other.MaxValue);
		}
		return res;
	}
	
	/// <summary>
	/// Returns true if this instance contains the given value.
	/// Returns false if either the min or max values are NaN.
	/// </summary>
	public bool Contains(float value) {
		return !float.IsNaN(this.MinValue) && !float.IsNaN(this.MaxValue) && value >= this.MinValue && value <= this.MaxValue;
	}

	public override string ToString() {
		return $"{this.MinValue}-{this.MaxValue}";
	}
}