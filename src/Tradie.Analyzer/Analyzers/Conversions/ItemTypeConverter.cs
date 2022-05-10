using Microsoft.Extensions.Logging;
using Tradie.Analyzer.Entities;
using Tradie.Common;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers.Conversions;

/// <summary>
/// Converter for handling conversions from a raw item to a parsed ItemType.
/// </summary>
public class ItemTypeConverter : IPersistentEntityConverter<ItemType> {
	public ItemTypeConverter(ILogger<ItemTypeConverter> logger) {
		this._logger = logger;
	}
	
	public ItemType ConvertFromRaw(Item item) {
		return new ItemType() {
			Category = item.ExtendedProperties?.Category,
			Subcategories = item.ExtendedProperties?.Subcategories ?? Array.Empty<string>(),
			Height = item.Height,
			Width = item.Width,
			Name = item.BaseType,
			Requirements = this.MapRequirements(item),
			IconUrl = item.IconPath
		};
	}
	
	public bool RequiresUpdate(ItemType mapped, ItemType incoming) {
		return new[] {
			mapped.IconUrl != incoming.IconUrl,
			!mapped.Subcategories.UnorderedSequenceEquals(incoming.Subcategories),
			mapped.Category != incoming.Category,
			mapped.Requirements != incoming.Requirements || !mapped.Requirements.Equals(incoming.Requirements)
		}.Any(c => c);
	}

	public ItemType MergeFrom(ItemType existing, ItemType incoming) {
		this._logger.LogInformation("Updating item type {@Existing} with properties from {@Incoming}", existing, incoming);
		existing.IconUrl = incoming.IconUrl;
		existing.Subcategories = incoming.Subcategories;
		existing.Category = incoming.Category;
		existing.Requirements = incoming.Requirements;
		return existing;
	}

	private Requirements? MapRequirements(Item item) {
		if(item.Requirements == null) {
			return new Requirements();
		}
		
		var res = new Requirements();
		foreach(var prop in item.Requirements) {
			if(prop.Values[0].DisplayType != 0) {
				// TODO: This is going to result in base types having the wrong requirements.
				// Because they're based off item not type.
				// We need to check for DisplayType 0 and only set requirements there.
				// And then for missing requirements, update them with future items.
				// We can do this in the future though.
				this._logger.LogWarning("Skipping assigning requirements to {Type} with property {Prop} due to being modified", item.BaseType, prop.Name);
				return null;
			}
			switch(prop.Name) {
				case "Level":
					res.Level = int.Parse(prop.Values[0].Value);
					break;
				case "Int":
					res.Int = int.Parse(prop.Values[0].Value);
					break;
				case "Dex":
					res.Dex = int.Parse(prop.Values[0].Value);
					break;
				case "Str":
					res.Str = int.Parse(prop.Values[0].Value);
					break;
				default:
					this._logger.LogWarning("Unknown stat {Stat} on item {Id}", prop.Name, item.Id);
					break;
			}
		}
		return res;
	}

	private readonly ILogger<ItemTypeConverter> _logger;
}