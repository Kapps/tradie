using Microsoft.EntityFrameworkCore;
using RestSharp.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers; 

/// <summary>
/// An analyzer that scans for new base types and records them as they come in.
/// </summary>
public class ItemTypeAnalyzer : IItemAnalyzer {
	private ILogger<ItemTypeAnalyzer> _logger;
	public static Guid Id { get; } = new Guid("6FCA53F9-D3C1-432F-A0CE-9F43EC449C36");

	public ItemTypeAnalyzer(ILogger<ItemTypeAnalyzer> logger, IItemTypeRepository repo) {
		this._repo = repo;
		this._logger = logger;
	}

	public async Task AnalyzeItems(AnalyzedItem[] items) {
		var distinctTypes = items.Select(c => c.RawItem)
			.DistinctBy(c => c.BaseType)
			.ToArray();
			
		var existingTypes = (await this._repo.LoadByNames(distinctTypes.Select(c=>c.BaseType).ToArray()))
			.ToDictionary(c=>c.Name!);
		var missingTypes = distinctTypes.Where(c => !existingTypes.ContainsKey(c.BaseType)).ToArray();

		var toInsert = new List<ItemType>();
		foreach(var missing in missingTypes) {
			if(missing.ExtendedProperties?.Subcategories?.Length > 1) {
				this._logger.LogWarning("Item with ID {ID} had subcategories {subcategories}, which is more than 1.", missing.Id, missing.ExtendedProperties?.Subcategories);
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
			await this._repo.Insert(toInsert);
		}
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
				this._logger.LogWarning("Inserting item type {type} with property {prop} being modified.", item.BaseType, prop.Name);
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
					this._logger.LogWarning("Unknown stat {stat} on item {id}", prop.Name, item.Id);
					break;
			}
		}
		return res;
	}

	private readonly IItemTypeRepository _repo;
}