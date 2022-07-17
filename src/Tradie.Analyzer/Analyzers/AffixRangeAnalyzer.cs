using System.Diagnostics.CodeAnalysis;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.Analyzer.Analyzers;

public class AffixRangeAnalyzer : IItemAnalyzer {
	public int Order => 100;
	
	public AffixRangeAnalyzer(IAffixRangeRepository repo) {
		this._repo = repo;
	}

	public async ValueTask AnalyzeItems(AnalyzedItem[] items) {
		var modDetails = items.Select(c=>c.Analysis.GetRequired<ItemAffixesAnalysis>(KnownAnalyzers.Modifiers)).ToArray();
		//var raw = items.Select(c => c.RawItem);
		var implicits = this.ExtractRanges(modDetails.SelectMany(c => c.Affixes)
			.Where(c => c.Kind == ModKind.Implicit || c.Kind == ModKind.Utility), ModKind.Implicit);
		var explicits = this.ExtractRanges(modDetails.SelectMany(c => c.Affixes)
			.Where(c => c.Kind == ModKind.Explicit || c.Kind == ModKind.Fractured || c.Kind == ModKind.Crafted || c.Kind == ModKind.Veiled), ModKind.Explicit);
		var enchants = this.ExtractRanges(modDetails.SelectMany(c => c.Affixes)
			.Where(c => c.Kind == ModKind.Enchant), ModKind.Enchant);
		var pseudo = this.ExtractRanges(modDetails.SelectMany(c => c.Affixes)
			.Where(c => c.Kind == ModKind.Pseudo), ModKind.Pseudo);
		var totals = this.ExtractTotalRange(items);
		
		var ranges = implicits.ConcatMany(explicits, enchants, pseudo, totals);

		await this._repo.UpsertRanges(ranges, CancellationToken.None);
	}

	private IEnumerable<AffixRange> ExtractRanges(IEnumerable<Affix> affixes, ModKind kind) {
		//var affixes = ModParser.ExtractAffixes(raw, kind);
		return affixes.GroupBy(c => c.Hash).Select(c => c.Aggregate(
			new AffixRange(c.Key, null, null, AffixRangeEntityKind.Modifier, kind.GetCategory()),
			(a, b) => a with {
				MinValue = Math.Min(a.MinValue ?? float.MaxValue, (float)b.Scalar),
				MaxValue = Math.Max(a.MaxValue ?? float.MinValue, (float)b.Scalar)
			})
		);
	}
	
	private IEnumerable<AffixRange> ExtractTotalRange(AnalyzedItem[] items) {
		var totalAffixes = items.SelectMany(item => {
			var modAnalysis = item.Analysis.GetRequired<ItemAffixesAnalysis>(KnownAnalyzers.Modifiers);
			return modAnalysis.Affixes
				.Where(affix => affix.Kind != ModKind.Pseudo)
				.GroupBy(affix => affix.Hash)
				.Select(group => new Affix(group.Key, group.Sum(affix => affix.Scalar), ModKind.Total));
		});
		return totalAffixes.GroupBy(affix => affix.Hash)
			.Select(group => group.Aggregate(
				new AffixRange(group.Key, null, null, AffixRangeEntityKind.Modifier, ModKindCategory.Pseudo),
				(a, b) => a with {
					MinValue = Math.Min(a.MinValue ?? float.MaxValue, (float)b.Scalar),
					MaxValue = Math.Max(a.MaxValue ?? float.MinValue, (float)b.Scalar)
				}));
	}

	public async ValueTask DisposeAsync() {
		await this._repo.DisposeAsync();
	}

	private readonly IAffixRangeRepository _repo;
}