using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.Common.RawModels;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers;

[TestClass]
public class TradeAttributesAnalyzerTests : TestBase {

	[TestMethod]
	public async Task TestEmpty() {
		var items = Array.Empty<AnalyzedItem>();
		await this._analyzer.AnalyzeItems(items);
		Assert.AreEqual(0, items.Length);
	}

	[TestMethod]
	public async Task TestBasic() {
		var item = new AnalyzedItem(await ItemUtils.ReadTestItem("boots"));
		await this._analyzer.AnalyzeItems(new[] { item });

		var props = (TradeListingAnalysis)item.Analysis[TradePropertiesAnalyzer.Id]!;
		Assert.IsNotNull(props);

		props.ShouldDeepEqual(new TradeListingAnalysis(
			8, 6,
			new ItemPrice(BuyoutCurrency.Chaos, 75, BuyoutKind.Fixed),
			"~price 75 chaos"
		));
	}

	[TestInitialize]
	public void Initializer() {
		this._analyzer = new TradePropertiesAnalyzer();
	}

	private TradePropertiesAnalyzer _analyzer = null!;
}
