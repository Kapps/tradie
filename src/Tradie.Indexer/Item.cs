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
	/// The list of affixes present on this item.
	/// </summary>
	[JsonInclude]
	public readonly Affix[] Affixes;

	public Item(string id, Affix[] affixes) {
		this.Id = id;
		this.Affixes = affixes;
	}

	public override string ToString() {
		return JsonSerializer.Serialize(this);
	}
}