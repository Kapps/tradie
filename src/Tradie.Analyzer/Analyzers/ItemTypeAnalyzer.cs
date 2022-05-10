using MessagePack;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers; 

/// <summary>
/// An analyzer that scans for new base types and records them as they come in.
/// </summary>
public class ItemTypeAnalyzer : IItemAnalyzer {
	private readonly ILogger<ItemTypeAnalyzer> _logger;
	public static ushort Id { get; } = KnownAnalyzers.ItemType;

	public ItemTypeAnalyzer(ILogger<ItemTypeAnalyzer> logger, IItemTypeRepository repo, IPersistentEntityConverter<ItemType> converter) {
		this._repo = repo;
		this._logger = logger;
		this._converter = converter;
	}

	public async ValueTask AnalyzeItems(AnalyzedItem[] items) {
		var mappedTypes = await this.MapTypesWithRepo(items);
		foreach(var item in items) {
			var mappedType = mappedTypes[item.RawItem.BaseType];
			// ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
			item.Analysis.PushAnalysis(Id, new ItemTypeAnalysis(mappedType.Id));
		}
	}

	private async Task<Dictionary<string, ItemType>> MapTypesWithRepo(AnalyzedItem[] items) {
		var parsedTypes = items.Select(c => c.RawItem)
			.DistinctBy(c => c.BaseType)
			.Select(c=> this._converter.ConvertFromRaw(c))
			.ToDictionary(c=>c.Name!);
			
		var existingTypes = (await this._repo.LoadByNames(parsedTypes.Keys.ToArray(), CancellationToken.None))
			.ToDictionary(c=>c.Name!);
		
		var missingTypes = parsedTypes.Where(c => !existingTypes.ContainsKey(c.Key)).Select(c=>c.Value)
			.ToArray();
		var typesToUpdate = existingTypes.Where(c => this._converter.RequiresUpdate(c.Value, parsedTypes[c.Value.Name!]))
			.Select(c=>this._converter.MergeFrom(c.Value, parsedTypes[c.Value.Name!]))
			.ToArray();

		if(missingTypes.Any()) {
			await this._repo.Insert(missingTypes, CancellationToken.None);
		}

		if(typesToUpdate.Any()) {
			await this._repo.Update(typesToUpdate, CancellationToken.None);
		}

		return existingTypes.Values.ToArray()
			.Concat(missingTypes)
			.ToDictionary(c => c.Name!);
	}

	private readonly IItemTypeRepository _repo;
	private readonly IPersistentEntityConverter<ItemType> _converter;

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