using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tradie.Analyzer.Analyzers;

namespace Tradie.Analyzer.Models; 

/// <summary>
/// The base type for an item, such as a Coral Amulet.
/// </summary>
[Table("ItemTypes")]
[Index(nameof(Name), IsUnique = true, Name = "idx_itemtype_name")]
public class ItemType : IAnalyzedProperties {
	public Guid Analyzer => ItemTypeAnalyzer.Id;

	/// <summary>
	/// Unique ID for the item type.
	/// </summary>
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Column]
	public int Id { get; set; }
	/// <summary>
	/// Name of the item type.
	/// </summary>
	[Column]
	public string? Name { get; set; }
	/// <summary>
	/// Category, such as amulet or gloves.
	/// </summary>
	[Column]
	public string? Category { get; set; }
	/// <summary>
	/// Subcategory, such as Claw or Abyss Jewel.
	/// </summary>
	[Column]
	public string? Subcategory { get; set; }
	/// <summary>
	/// Requirements to be able to use this item.
	/// </summary>
	[Column]
	[Required]
	public Requirements? Requirements { get; set; }
	/// <summary>
	/// Amount of inventory slots items of this type take up horizontally.
	/// </summary>
	[Column]
	public int Width { get; set; }
	/// <summary>
	/// Amount of inventory slots items of this type take up vertically.
	/// </summary>
	[Column]
	public int Height { get; set; }

}