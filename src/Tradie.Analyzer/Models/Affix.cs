using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer.Models; 

/// <summary>
/// Represents the value of a modifier on an item, including scalar and location or kind.
/// </summary>
[DataContract]
public readonly record struct Affix {
	[DataMember(Name = "hash", Order = 1, IsRequired = true)]
	[JsonInclude]
	[JsonPropertyName("hash")]
	public readonly ulong Hash;

	[DataMember(Name = "scalar", Order = 2, IsRequired = true)]
	[JsonInclude]
	[JsonPropertyName("scalar")]
	public readonly double Scalar;

	[DataMember(Name = "kind", Order = 3, IsRequired = true)]
	[JsonInclude]
	[JsonPropertyName("kind")]
	[JsonConverter(typeof(FlagsEnumJsonConverter<ModKind>))]
	public readonly ModKind Kind;

	[SpanJson.JsonConstructor]
	[JsonConstructor]
	public Affix(ulong hash, double scalar, ModKind kind) {
		this.Hash = hash;
		this.Scalar = double.IsNaN(scalar) ? 0 : scalar; // JSON doesn't support NaN
		this.Kind = kind;
	}
}