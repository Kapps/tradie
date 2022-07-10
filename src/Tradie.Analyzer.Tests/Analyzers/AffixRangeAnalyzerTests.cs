using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers;

[TestClass]
public class AffixRangeAnalyzerTests : TestBase {
	protected override void Initialize() {
		this._analyzer = new AffixRangeAnalyzer(this._repo.Object);
	}

	[TestMethod]
	public async Task TestAnalyze() {
		var item = new AnalyzedItem(await ItemUtils.ReadTestItem("rangetest"));
		item.Analysis.PushAnalysis(KnownAnalyzers.Modifiers, new ItemAffixesAnalysis(new Affix[] {
			new(ModifierText.CalculateValueIndependentHash("20% increased Movement Speed"), 20, ModKind.Explicit),
			new(ModifierText.CalculateValueIndependentHash("+46% to Cold Resistance"), 46, ModKind.Explicit),
			new(ModifierText.CalculateValueIndependentHash("+23% to Cold Resistance"), 23, ModKind.Crafted),
			new(ModifierText.CalculateValueIndependentHash("Immune to Freeze"), 0, ModKind.Explicit),
			new(ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"), 10, ModKind.Enchant),
			new(ModifierText.CalculateValueIndependentHash("20% increased Movement Speed"), 20, ModKind.Scourge),
			new(PseudoMods.TotalEleRes.ModHash, 69, ModKind.Pseudo)
		}, 2, 2));
		
		var expectedRanges = new AffixRange[] {
			new(
				ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"),
				20, 20, AffixRangeEntityKind.Modifier, ModKindCategory.Explicit
			),
			new(
				ModifierText.CalculateValueIndependentHash("+23% to Cold Resistance"),
				23, 46, AffixRangeEntityKind.Modifier, ModKindCategory.Explicit
			),
			new(
				ModifierText.CalculateValueIndependentHash("Immune to Freeze"),
				0, 0, AffixRangeEntityKind.Modifier, ModKindCategory.Explicit
			),
			new(
				ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"),
				10, 10, AffixRangeEntityKind.Modifier, ModKindCategory.Enchant
			),
			new(
				PseudoMods.TotalEleRes.ModHash, 69, 69, AffixRangeEntityKind.Modifier, ModKindCategory.Pseudo
			)
		};

		this._repo.Setup(c => c.UpsertRanges(expectedRanges.DeepMatcher<IEnumerable<AffixRange>>(), CancellationToken.None))
			.Returns(Task.CompletedTask);

		await this._analyzer.AnalyzeItems(new[] { item });
	}

	private AffixRangeAnalyzer _analyzer = null!;
	private Mock<IAffixRangeRepository> _repo = null!;
}