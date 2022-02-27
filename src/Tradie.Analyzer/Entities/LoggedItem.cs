using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// An analyzed item being persisted within the parent stash tab to be used as an item log entry.
/// </summary>
public record struct LoggedItem(string RawId, Dictionary<ushort, IAnalyzedProperties> Properties) {
	/// <summary>
	/// The raw Path of Exile ID of the item, as it is in the PoE API.
	/// </summary>
	[Required]
	public string RawId { get; set; } = RawId ?? throw new ArgumentNullException(nameof(RawId));

	/// <summary>
	/// The analyzed properties of this item, stored as a Jsonb blob.
	/// </summary>
	[Required]
	[DefaultValue("{}")]
	public Dictionary<ushort, IAnalyzedProperties> Properties { get; set; } = Properties ?? throw new ArgumentNullException(nameof(Properties));
}

