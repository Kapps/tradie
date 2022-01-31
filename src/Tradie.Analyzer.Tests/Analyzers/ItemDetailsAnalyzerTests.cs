using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers;

[TestClass]
public class ItemDetailsAnalyzerTests : TestBase {

	[TestMethod]
	public async Task TestParse() {
		var item = await ItemUtils.ReadTestItem("influenced");
		var analyzer = new AnalyzedItem(item);
		
		await this._analyzer.AnalyzeItems(new[] {analyzer});

		var props = analyzer.Analysis[ItemDetailsAnalyzer.Id];
		props.ShouldDeepEqual(new ItemDetailsAnalysis(
			"Behemoth March",
			ItemFlags.Corrupted | ItemFlags.Synthesized,
			InfluenceKind.Crusader | InfluenceKind.Shaper,
			67
		));
	}

	[TestInitialize]
	public void Initializer() {
		this._analyzer = new ItemDetailsAnalyzer();
	}

	private ItemDetailsAnalyzer _analyzer = null!;
}