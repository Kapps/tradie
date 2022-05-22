using System.Diagnostics.CodeAnalysis;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.Analyzer.Analyzers;

public class AffixRangeAnalyzer : IItemAnalyzer {
	public AffixRangeAnalyzer(IAffixRangeRepository repo) {
		this._repo = repo;
	}

	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	public async ValueTask AnalyzeItems(AnalyzedItem[] items) {
		var raw = items.Select(c => c.RawItem);
		var implicits = this.ExtractRanges(raw.SelectMany(c => c.ImplicitMods.ConcatMany(c.UtilityMods)), ModKind.Implicit);
		var explicits = this.ExtractRanges(raw.SelectMany(c => c.ExplicitMods.ConcatMany(c.FracturedMods, c.VeiledMods, c.CraftedMods)),
			ModKind.Explicit);
		var enchants = this.ExtractRanges(raw.SelectMany(c => c.EnchantMods ?? Array.Empty<string>()), // Skip cosmetics
			ModKind.Enchant);

		var ranges = implicits.ConcatMany(explicits, enchants);

		await this._repo.UpsertRanges(ranges, CancellationToken.None);
	}

	private IEnumerable<AffixRange> ExtractRanges(IEnumerable<string> raw, ModKind kind) {
		var affixes = ModParser.ExtractAffixes(raw, kind);
		return affixes.GroupBy(c => c.Hash).Select(c => c.Aggregate(
			new AffixRange(c.Key, null, null, AffixRangeEntityKind.Modifier, kind.GetCategory()),
			(a, b) => a with {
				MinValue = Math.Min(a.MinValue ?? float.MaxValue, (float)b.Scalar), 
				MaxValue = Math.Max(a.MaxValue ?? float.MinValue, (float)b.Scalar)
			})
		);
	}

	public async ValueTask DisposeAsync() {
		await this._repo.DisposeAsync();
	}

	private readonly IAffixRangeRepository _repo;
}