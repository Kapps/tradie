using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
		var items = await Task.WhenAll(new[] { "rangetest" }.Select(ItemUtils.ReadTestItem));

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
				ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"),
				10, 10, AffixRangeEntityKind.Modifier, ModKindCategory.Enchant
			),
			new(
				ModifierText.CalculateValueIndependentHash("Immune to Freeze"),
				null, null, AffixRangeEntityKind.Modifier, ModKindCategory.Explicit
			),

		};

		this._repo.Setup(c => c.UpsertRanges(expectedRanges.DeepMatcher<IEnumerable<AffixRange>>(), CancellationToken.None))
			.Returns(Task.CompletedTask);

		await this._analyzer.AnalyzeItems(items.Select(c => new AnalyzedItem(c)).ToArray());
	}

	private AffixRangeAnalyzer _analyzer = null!;
	private Mock<IAffixRangeRepository> _repo = null!;
}