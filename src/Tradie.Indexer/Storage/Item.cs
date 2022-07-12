using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;

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

	/// <summary>
	/// Returns the value of the given modifier on this item, or zero if the modifier is not found.
	/// </summary>
	public float GetAffixValue(ModKey modifier) {
		// TODO: This should binary search.

		if(modifier.Kind == ModKind.Total) {
			float sum = 0;
			foreach(var affix in this.Affixes) {
				if(affix.Modifier.ModHash == modifier.ModHash)
					sum += affix.Value;
			}

			return sum;
		}
		
		foreach(var affix in this.Affixes) {
			if(affix.Modifier == modifier)
				return affix.Value;
		}

		return 0;
	}

	/// <summary>
	/// Returns the affix with the given key, or null if none was found.
	/// In the case of a total modifier kind, it is not defined which affix will be returned if multiple match.
	/// </summary>
	public Affix? FindAffix(ModKey modifier) {
		// TODO: This should binary search.
		if(modifier.Kind == ModKind.Total) {
			foreach(var affix in this.Affixes) {
				if(affix.Modifier.ModHash == modifier.ModHash)
					return affix;
			}

			return null;
		}
		foreach(var affix in this.Affixes) {
			if(affix.Modifier == modifier)
				return affix;
		}

		return null;
	}
	
	int IComparable<Item>.CompareTo(Item other) {
		if(Math.Abs(this.ChaosEquivalentPrice - other.ChaosEquivalentPrice) < 0.0001) {
			return String.Compare(this.Id, other.Id, StringComparison.Ordinal);
		}

		return this.ChaosEquivalentPrice.CompareTo(other.ChaosEquivalentPrice);
	}
}