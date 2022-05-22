using MessagePack;
using System.Runtime.Serialization;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Models;

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

	public async ValueTask AnalyzeItems(AnalyzedItem[] items) {
		await this._converter.ConvertModifiers(items.Select(c => c.RawItem));
		
		foreach(var item in items) {
			var affixes = this._converter.ExtractAffixes(item.RawItem);
			byte prefixCount = (byte)(item.RawItem.ExtendedProperties?.Prefixes ?? 0);
			byte suffixCount = (byte)(item.RawItem.ExtendedProperties?.Suffixes ?? 0);
			//var props = new ItemAffixesAnalysis(affixes.ToDictionary(c=>new ModKey(c.Hash, c.Kind)));
			var props = new ItemAffixesAnalysis(affixes.ToArray(), prefixCount, suffixCount);
			item.Analysis.PushAnalysis(Id, props);
		}
	}

	public ValueTask DisposeAsync() {
		return this._converter.DisposeAsync();
	}

	private readonly IModConverter _converter;
}

/// <summary>
/// A set of analyzed properties to view all of the affixes on an item.
/// </summary>
/// <param name="Affixes">All affixes that are on this item, including implicit or explicit.</param>
/// <param name="PrefixCount">The number of explicit affixes which are prefixes.</param>
/// <param name="SuffixCount">The number of explicit affixes which are prefixes.</param>
[DataContract]
[MessagePackObject]
public readonly record struct ItemAffixesAnalysis(
	[property:DataMember,Key(0)] Affix[] Affixes,
	[property:DataMember,Key(1)] byte PrefixCount,
	[property:DataMember,Key(2)] byte SuffixCount
) : IAnalyzedProperties;

/// <summary>
/// The key to identify an affix on an item; aka the mod hash and the location of the affix.
/// </summary>
/// <param name="ModHash">The hash of the mod, as defined on Affix.</param>
/// <param name="Kind">The location of this affix; the same mod may be present multiple times in different locations.</param>
public readonly record struct ModKey(ulong ModHash, ModKind Kind);