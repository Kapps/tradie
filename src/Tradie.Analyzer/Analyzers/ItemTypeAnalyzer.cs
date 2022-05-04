using MessagePack;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers; 

/// <summary>
/// An analyzer that scans for new base types and records them as they come in.
/// </summary>
public class ItemTypeAnalyzer : IItemAnalyzer {
	private readonly ILogger<ItemTypeAnalyzer> _logger;
	public static ushort Id { get; } = KnownAnalyzers.ItemType;

	public ItemTypeAnalyzer(ILogger<ItemTypeAnalyzer> logger, IItemTypeRepository repo) {
		this._repo = repo;
		this._logger = logger;
	}

	public async ValueTask AnalyzeItems(AnalyzedItem[] items) {
		var mappedTypes = await this.MapTypesWithRepo(items);
		foreach(var item in items) {
			var mappedType = mappedTypes[item.RawItem.BaseType];
			item.Analysis.PushAnalysis(Id, new ItemTypeAnalysis(mappedType.Id));
		}
	}

	private async Task<Dictionary<string, ItemType>> MapTypesWithRepo(AnalyzedItem[] items) {
		var distinctTypes = items.Select(c => c.RawItem)
			.DistinctBy(c => c.BaseType)
			.ToArray();
			
		var existingTypes = (await this._repo.LoadByNames(distinctTypes.Select(c=>c.BaseType).ToArray(), CancellationToken.None))
			.ToDictionary(c=>c.Name!);
		var missingTypes = distinctTypes.Where(c => !existingTypes.ContainsKey(c.BaseType)).ToArray();

		var toInsert = new List<ItemType>();
		foreach(Item missing in missingTypes) {
			if(missing.ExtendedProperties?.Subcategories?.Length > 1) {
				this._logger.LogWarning("Item with ID {ID} had subcategories {Subcategories}, which is more than 1",
					missing.Id, missing.ExtendedProperties?.Subcategories);
			}
			var converted = new ItemType() {
				Category = missing.ExtendedProperties?.Category,
				Subcategory = missing.ExtendedProperties?.Subcategories?[0],
				Height = missing.Height,
				Width = missing.Width,
				Name = missing.BaseType,
				Requirements = this.MapRequirements(missing),
			};
			toInsert.Add(converted);
		}

		if(toInsert.Any()) {
			await this._repo.Insert(toInsert, CancellationToken.None);
		}

		return existingTypes.Values.ToArray()
			.Concat(toInsert)
			.ToDictionary(c => c.Name!);
	}

	private Requirements MapRequirements(Item item) {
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
				this._logger.LogWarning("Inserting item type {Type} with property {Prop} being modified", item.BaseType, prop.Name);
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

	private readonly IItemTypeRepository _repo;
	public async ValueTask DisposeAsync() {
		await this._repo.DisposeAsync();
	}
}

/// <summary>
/// A set of analyzed properties to view the attributes for the base type of an item.
/// </summary>
/// <param name="ItemTypeId">Unique ID of the item type in the analysis database.</param>
[DataContract, MessagePackObject]
public readonly record struct ItemTypeAnalysis(
	[property:DataMember,Key(0)] int ItemTypeId
) : IAnalyzedProperties;