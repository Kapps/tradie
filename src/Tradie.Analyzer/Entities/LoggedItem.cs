using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// An analyzed item being persisted within the context to be used as an item log entry.
/// </summary>
[Table("Items")]
public class LoggedItem {
	/// <summary>
	/// An auto-generated serial entity ID for this item.
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// The raw Path of Exile ID of the item, as it is in the PoE API.
	/// </summary>
	[Column]
	[Required]
	public string RawId { get; set; }

	/// <summary>
	/// The entity ID of the stash tab containing this item.
	/// </summary>
	[Column]
	[Required]
	public long StashTabId { get; set; }

	/// <summary>
	/// The name of this item, such as the random name on a rare item.
	/// </summary>
	[Column]
	[Required]
	public string Name { get; set; }

	/// <summary>
	/// The item level of the item, if applicable.
	/// </summary>
	[Column]
	public int? ItemLevel { get; set; }

	/// <summary>
	/// The note, which is often a list or buyout price.
	/// </summary>
	[Column]
	public string Note { get; set; }

	/// <summary>
	/// The X coordinate within the stash tab for the top-left of this item.
	/// </summary>
	[Column]
	[Required]
	public ushort X { get; set; }
	
	/// <summary>
	/// The Y coordinate within the stash tab for the top-left of this item.
	/// </summary>
	[Column]
	[Required]
	public ushort Y { get; set; }

	/// <summary>
	/// Any flags that apply to this item, such as being corrupted.
	/// </summary>
	[Column]
	[Required]
	[DefaultValue(ItemFlags.None)]
	public ItemFlags Flags { get; set; }

	/// <summary>
	/// The type of influences, if any, that apply to this item.
	/// </summary>
	[Column]
	[Required]
	[DefaultValue(InfluenceKind.None)]
	public InfluenceKind Influences { get; set; }
}

/// <summary>
/// Additional flags that apply to an item, such as being corrupted.
/// </summary>
[Flags]
public enum ItemFlags {
	None = 0,
	Corrupted = 1,
	Mirrored = 2,
	Veiled = 4,
	Relic = 8,
	Replica = 16,
	Synthesized = 32,
	Fractured = 64
}

/// <summary>
/// The types of influences present on an item.
/// </summary>
[Flags]
public enum InfluenceKind {
	None = 0,
	Redeemer = 1,
	Crusader = 2,
	Warlord = 4,
	Hunter = 8,
	Shaper = 16,
	Elder = 32
}