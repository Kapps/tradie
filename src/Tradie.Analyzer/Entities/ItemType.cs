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
public class ItemType {
	public ItemType() { }


	public ItemType(int id, string? name, string? category, string[] subcategories, Requirements? requirements, string iconUrl, int width, int height) {
		this.Id = id;
		this.Name = name;
		this.Category = category;
		this.Subcategories = subcategories;
		this.Requirements = requirements;
		this.Width = width;
		this.Height = height;
		this.IconUrl = iconUrl;
	}

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
	public string[]? Subcategories { get; set; }
	/// <summary>
	/// Requirements to be able to use this item.
	/// </summary>
	[Column]
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
	/// <summary>
	/// The absolute web URL to an icon for this item type. 
	/// </summary>
	[Column]
	public string? IconUrl { get; set; }


	public override string ToString() {
		return
			$"{nameof(this.Id)}: {this.Id}, {nameof(this.Name)}: {this.Name}, {nameof(this.Category)}: {this.Category}, " +
			$"{nameof(this.Subcategories)}: {String.Join(", ", this.Subcategories ?? Array.Empty<string>())}, " +
			$"{nameof(this.Requirements)}: {this.Requirements}, {nameof(this.Width)}: {this.Width}, " +
			$"{nameof(this.Height)}: {this.Height}, {nameof(this.IconUrl)}: {this.IconUrl}";
	}
}