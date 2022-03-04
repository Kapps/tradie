using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// The base type for an item, such as a Coral Amulet.
/// </summary>
[Table("ItemTypes")]
[Index(nameof(Name), IsUnique = true, Name = "idx_itemtype_name")]
[Index(nameof(Category), Name = "idx_itemtype_category")]
[Index(nameof(Subcategory), Name = "idx_itemtype_subcategory")]
public class ItemType {
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