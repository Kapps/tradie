using SpanJson;
using System.Runtime.Serialization;
using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer.Models; 

/// <summary>
/// The type of mod that an affix represents.
/// </summary>
public enum ModKind : byte {
	Unknown = 0,
	Implicit = 1,
	Explicit = 2,
	Enchant = 3,
	Scourge = 4,
	Utility = 5,
	Fractured = 6,
	Cosmetic = 7,
	Veiled = 8,
}


/// <summary>
/// Represents the value of a modifier on an item, including scalar and location or kind.
/// </summary>
[DataContract]
public readonly record struct Affix {
	[DataMember(Name = "hash", Order = 1, IsRequired = true)]
	public readonly ulong Hash;

	[DataMember(Name = "scalar", Order = 2, IsRequired = true)]
	public readonly double Scalar;

	[DataMember(Name = "kind", Order = 3, IsRequired = true)]
	public readonly ModKind Kind;

	[JsonConstructor]
	public Affix(ulong hash, double scalar, ModKind kind) {
		this.Hash = hash;
		this.Scalar = double.IsNaN(scalar) ? 0 : scalar; // JSON doesn't support NaN
		this.Kind = kind;
	}
}