using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// An analyzed item being persisted within the parent stash tab to be used as an item log entry.
/// </summary>
[Table("Items")]
[Index(nameof(IdHash), IsUnique = true)]
[Index(nameof(StashTabId))]
[Index(nameof(RawId))]
//[JsonConverter(typeof(LoggedItemJsonConverter))]
public record class LoggedItem(string RawId, long StashTabId, AnalyzedPropertyCollection Properties) {
	/// <summary>
	/// The raw Path of Exile ID of the item, as it is in the PoE API.
	/// </summary>
	[Required]
	[Column]
	public string RawId { get; set; } = RawId ?? throw new ArgumentNullException(nameof(RawId));

	/// <summary>
	/// A hash of the raw ID, used for more efficient lookups.
	/// </summary>
	[Required]
	[Key]
	[Column(TypeName = "bigint")]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public ulong IdHash { get; set; } = GenerateIdHash(RawId);

	/// <summary>
	/// Database ID of the stash tab currently containing this item.
	/// </summary>
	[Required]
	[Column]
	public long StashTabId { get; set; } = StashTabId;

	/// <summary>
	/// The analyzed properties of this item, stored as a Jsonb blob.
	/// </summary>
	[Required]
	[DefaultValue("{}")]
	[Column(TypeName = "jsonb")]
	public AnalyzedPropertyCollection Properties { get; set; } = Properties;

	/// <summary>
	/// Generates a consistent 64 bit hash for a raw item ID.
	/// </summary>
	public static unsafe ulong GenerateIdHash(string rawId) {
		const ulong fnv64Offset = 14695981039346656037;
		const ulong fnv64Prime = 1099511628211;
		ulong hash = fnv64Offset;
		fixed(void* _ptr = rawId) {
			byte* ptr = (byte*)_ptr;
			for(int i = 0; i < rawId.Length * 2; i += 2) {
				// Only care about the ASCII characters as we don't have non-ASCII mods.
				byte b = ptr[i];
				hash ^= b;
				hash *= fnv64Prime;
			}
		}

		return hash;
	}
}

