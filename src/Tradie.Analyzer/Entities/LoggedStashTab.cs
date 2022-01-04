using Microsoft.EntityFrameworkCore;
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
[Index(nameof(League), IsUnique = true, Name = "idx_stash_League")]
public class LoggedStashTab {
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
	/// Time that this tab was first seen, in UTC.
	/// </summary>
	[Column]
	[Required]
	public DateTime Created { get; set; }

	/// <summary>
	/// The account that owns this tab.
	/// </summary>
	[Column]
	[Required]
	public string Owner { get; set; }

	/// <summary>
	/// The character name the account was last seen using.
	/// </summary>
	[Column]
	public string? LastCharacterName { get; set; }
	
	/// <summary>
	/// Name of this tab.
	/// </summary>
	[Column]
	[Required]
	public string Name { get; set; }
	
	/// <summary>
	/// Name of the league this tab is in.
	/// </summary>
	[Column]
	[Required]
	public string League { get; set; }
}