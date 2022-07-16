using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tradie.Analyzer.Entities;

/// <summary>
/// A single possible modifier on an item, regardless of whether unique or rare, implicit or explicit.
/// </summary>
[Table("Modifiers")]
[Index(nameof(ModHash), IsUnique = true, Name = "idx_modifier_modhash")]
[Index(nameof(ModifierText), IsUnique = false, Name = "idx_modifier_modtext")]	
public class Modifier {
	/// <summary>
	/// An auto-generated unique identifier for the modifier.
	/// </summary>
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Column]
	public int Id { get; set; }
	
	/// <summary>
	/// A value independent hash of the text for this modifier. 
	/// </summary>
	[Column]
	[Required]
	public ulong ModHash { get; set; }
	
	/// <summary>
	/// The original text of the modifier, with values replaced.
	/// </summary>
	[Column]
	[Required]
	public string? ModifierText { get; set; }
	
	/// <summary>
	/// Indicates the type of modifier this is.
	/// </summary>
	[Column]
	[Required]
	[DefaultValue(ModifierKind.Standard)]
	public ModifierKind Kind { get; set; }

	public Modifier() { }

	public Modifier(ulong modHash, string modifierText, ModifierKind kind = ModifierKind.Standard) {
		this.ModHash = modHash;
		this.ModifierText = modifierText ?? throw new ArgumentNullException(nameof(modifierText));
		this.Kind = kind;
	}
}

/// <summary>
/// Indicates the type of a modifier, whether a standard modifier or a pseudo modifier. 
/// </summary>
public enum ModifierKind {
	Standard = 0,
	Pseudo = 1
}