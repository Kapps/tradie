using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Indexer.Storage;

/// <summary>
/// Compact representation of an item, containing only its id and affixes.
/// </summary>
public readonly struct Item : IComparable<Item> {
	// TODO: Should Id be a Guid or something at least to reduce data volume?
	
	/// <summary>
	/// The unique identifier for the item.
	/// </summary>
	[JsonInclude]
	public readonly string Id;
	/// <summary>
	/// The price of this item, in chaos-equivalent orbs.
	/// </summary>
	[JsonInclude]
	public readonly float ChaosEquivalentPrice;
	/// <summary>
	/// The list of affixes present on this item.
	/// </summary>
	[JsonInclude]
	public readonly Affix[] Affixes;

	public Item(string id, float chaosEquivalentPricePrice, Affix[] affixes) {
		this.Id = id;
		this.Affixes = affixes;
		this.ChaosEquivalentPrice = (float)Math.Round(chaosEquivalentPricePrice,2);
	}

	public override string ToString() {
		return JsonSerializer.Serialize(this, new JsonSerializerOptions() {
			WriteIndented = true
		});
	}

	public Affix? FindAffix(ModKey modifier) {
		foreach(var affix in this.Affixes) {
			if(affix.Modifier == modifier)
				return affix;
		}

		return null;
	}
	
	int IComparable<Item>.CompareTo(Item other) {
		if(Math.Abs(this.ChaosEquivalentPrice - other.ChaosEquivalentPrice) < 0.01) {
			return String.Compare(this.Id, other.Id, StringComparison.Ordinal);
		}

		return this.ChaosEquivalentPrice.CompareTo(other.ChaosEquivalentPrice);
	}
}