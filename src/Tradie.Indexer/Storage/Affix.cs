using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;

namespace Tradie.Indexer.Storage;

/// <summary>
/// A compact representation of an affix on an item, containing the hash, the value, and the location.
/// </summary>
public readonly struct Affix {
	/// <summary>
	/// The hash value and location of the modifier to look up the affix.
	/// </summary>
	[JsonInclude]
	public readonly ModKey Modifier;
	/// <summary>
	/// The scalar value of the affix.
	/// </summary>
	[JsonInclude]
	public readonly float Value;

	public Affix(ModKey modifier, float value) {
		this.Modifier = modifier;
		this.Value = value;
	}
}