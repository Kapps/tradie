using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tradie.Indexer;

/// <summary>
/// Compact representation of an item, containing only its id and affixes.
/// </summary>
public readonly struct Item {
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
	public readonly int ChaosEquivalent;
	/// <summary>
	/// The list of affixes present on this item.
	/// </summary>
	[JsonInclude]
	public readonly Affix[] Affixes;

	public Item(string id, int chaosEquivalent, Affix[] affixes) {
		this.Id = id;
		this.Affixes = affixes;
		this.ChaosEquivalent = chaosEquivalent;
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
}