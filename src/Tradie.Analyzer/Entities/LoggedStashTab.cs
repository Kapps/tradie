using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// Details of a stash tab that has been analyzed and is being stored within the context as part of an item log.
/// </summary>
[Table("StashTabs")]
[Index(nameof(RawId), IsUnique = true, Name = "idx_stash_RawId")]
[Index(nameof(LastModified), IsUnique = false, Name = "idx_stash_LastModified")]
[Index(nameof(Created), IsUnique = false, Name = "idx_stash_Created")]
[Index(nameof(League), IsUnique = false, Name = "idx_stash_League")]
public class LoggedStashTab {
	public LoggedStashTab(string rawId, DateTime lastModified, DateTime created, string? owner,
		string? lastCharacterName, string? name, string? league, string? kind, LoggedItem[] items) {
		this.RawId = rawId ?? throw new ArgumentNullException(nameof(rawId));
		this.LastModified = lastModified;
		this.Created = created;
		this.Owner = owner;
		this.LastCharacterName = lastCharacterName;
		this.Name = name;
		this.League = league;
		this.Kind = kind;
		this.Items = items;
	}

	/// <summary>
	/// Unique serial ID of this entity.
	/// </summary>
	[Column]
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }

	/// <summary>
	/// Raw PoE ID of this stash tab, as shown in the API.
	/// </summary>
	[Column]
	[Required]
	public string RawId { get; set; }
	
	/// <summary>
	/// Last time this tab had a change logged; defaults to creation time in UTC.
	/// </summary>
	[Column]
	[Required]
	public DateTime LastModified { get; set; }
	
	/// <summary>
	/// Time that this tab was first seen, in UTC; defaults to transaction time in UTC.
	/// </summary>
	[Column]
	[Required]
	public DateTime Created { get; set; }

	/// <summary>
	/// The account that owns this tab, if it's public; null otherwise.
	/// </summary>
	[Column]
	public string? Owner { get; set; }

	/// <summary>
	/// The character name the account was last seen using.
	/// </summary>
	[Column]
	public string? LastCharacterName { get; set; }
	
	/// <summary>
	/// Name of this tab, if it's public; may be null otherwise?
	/// </summary>
	[Column]
	public string? Name { get; set; }
	
	/// <summary>
	/// Name of the league this tab is in if public.
	/// </summary>
	[Column]
	public string? League { get; set; }

	/// <summary>
	/// The type of the stash tab, such as currency.
	/// </summary>
	[Column]
	public string? Kind { get; set; }

	[Column(TypeName = "jsonb")]
	[Required]
	[DefaultValue("[]")]
	public LoggedItem[] Items { get; set; }
	
	/// <summary>
	/// Optimized binary version of the items, stored with MessagePack and compressed via lz4.
	/// </summary>
	[Column(TypeName = "bytea")]
	public byte[] PackedItems { get; set; }
}