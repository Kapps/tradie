using Nest;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Tradie.Analyzer.Models; 

/// <summary>
/// Represents the value of a modifier on an item, including scalar and location or kind.
/// </summary>
[DataContract]
public readonly record struct Affix : IComparable<Affix> {
	[DataMember(Name = "hash", Order = 1, IsRequired = true)]
	[JsonInclude]
	[JsonPropertyName("hash")]
	[Number(NumberType.UnsignedLong)]
	public ulong Hash { get; init; }

	[DataMember(Name = "scalar", Order = 2, IsRequired = true)]
	[JsonInclude]
	[JsonPropertyName("scalar")]
	[Number(NumberType.Double)]
	public double Scalar { get; init; }

	[DataMember(Name = "kind", Order = 3, IsRequired = true)]
	[JsonInclude]
	[JsonPropertyName("kind")]
	[JsonConverter(typeof(FlagsEnumJsonConverter<ModKind>))]
	public ModKind Kind { get; init; }

	[SpanJson.JsonConstructor]
	[JsonConstructor]
	public Affix(ulong hash, double scalar, ModKind kind) {
		this.Hash = hash;
		this.Scalar = double.IsNaN(scalar) ? 0 : scalar; // JSON doesn't support NaN
		this.Kind = kind;
	}

	public int CompareTo(Affix other) {
		var hashComparison = this.Hash.CompareTo(other.Hash);
		if (hashComparison != 0) {
			return hashComparison;
		}

		var scalarComparison = this.Scalar.CompareTo(other.Scalar);
		if (scalarComparison != 0) {
			return scalarComparison;
		}

		return this.Kind.CompareTo(other.Kind);
	}
}