using MessagePack;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers; 

/// <summary>
/// An item analyzer that scans for affixes within items, keeping track of all available
/// affixes and efficiently adding them to the analysis of each item.
/// </summary>
public class ModifierAnalyzer : IItemAnalyzer {
	/// <summary>
	/// Unique ID of this analyzer.
	/// </summary>
	public static ushort Id { get; } = (ushort)KnownAnalyzers.Modifiers;

	/// <summary>
	/// Creates a new analyzer that uses the given converter to transform raw modifiers.
	/// </summary>
	public ModifierAnalyzer(IModConverter converter) {
		this._converter = converter ?? throw new ArgumentNullException(nameof(converter));
	}

	public async Task AnalyzeItems(AnalyzedItem[] items) {
		await this._converter.ConvertModifiers(items.Select(c => c.RawItem));
		
		foreach(var item in items) {
			var affixes = this._converter.ExtractAffixes(item.RawItem);
			var props = new ItemAffixesAnalysis(affixes.ToArray());
			item.Analysis.PushAnalysis(Id, props);
		}
	}

	public ValueTask DisposeAsync() {
		return ValueTask.CompletedTask;
	}

	private readonly IModConverter _converter;
}

/// <summary>
/// A set of analyzed properties to view all of the affixes on an item.
/// </summary>
/// <param name="Affixes">All affixes that are on this item, including implicit or explicit.</param>
[DataContract]
[MessagePackObject]
public readonly record struct ItemAffixesAnalysis(
	[property:DataMember,Key(0)] Affix[] Affixes
) : IAnalyzedProperties;