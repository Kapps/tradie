using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// An analyzed item being persisted within the context to be used as an item log entry.
/// </summary>
[Table("Items")]
[Index(nameof(RawId), IsUnique = true, Name = "idx_item_RawId")]
[Index(nameof(StashTabId), IsUnique = false, Name = "idx_item_StashTabId")]
public class LoggedItem {
	public LoggedItem(string rawId, long stashTabId, byte[] properties) {
		this.RawId = rawId ?? throw new ArgumentNullException(nameof(rawId));
		this.StashTabId = stashTabId;
		this.Properties = properties ?? throw new ArgumentNullException(nameof(properties));
	}

	/// <summary>
	/// An auto-generated serial entity ID for this item.
	/// </summary>
	[Column]
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
	/// The analyzed properties of this item, stored as a Jsonb blob.
	/// </summary>
	[Column(TypeName = "jsonb")]
	[Required]
	[DefaultValue("{}")]
	public byte[] Properties { get; set; }
}

