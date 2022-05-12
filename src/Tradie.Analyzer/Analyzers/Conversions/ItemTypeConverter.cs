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
			Requirements = RequirementParser.MapRequirements(item, false),
			IconUrl = item.FrameType == 3 ? null : item.IconPath,
		};
	}
	
	public bool RequiresUpdate(ItemType mapped, ItemType incoming) {
		return new[] {
			String.IsNullOrWhiteSpace(mapped.IconUrl) && !String.IsNullOrWhiteSpace(incoming.IconUrl),
			(incoming.Subcategories?.Length ?? 0) > (mapped.Subcategories?.Length ?? 0)
		}.Any(c=>c);
		/*return new[] {
			mapped.IconUrl != incoming.IconUrl,
			!mapped.Subcategories.UnorderedSequenceEquals(incoming.Subcategories),
			mapped.Category != incoming.Category,
			mapped.Category != "gems" && mapped.Requirements == default && !mapped.Requirements.Equals(incoming.Requirements)
		}.Any(c => c);*/
	}

	public ItemType MergeFrom(ItemType existing, ItemType incoming) {
		this._logger.LogInformation("Updating item type {Existing} with properties from {Incoming}", 
			existing.ToString(), incoming.ToString());
		existing.IconUrl = incoming.IconUrl;
		existing.Subcategories = incoming.Subcategories;
		existing.Category = incoming.Category;
		existing.Requirements = incoming.Requirements;
		return existing;
	}

	

	private readonly ILogger<ItemTypeConverter> _logger;
}