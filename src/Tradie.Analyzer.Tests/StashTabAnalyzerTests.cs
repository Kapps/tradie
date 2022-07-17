using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tradie.Analyzer.Tests.Analyzers;
using Tradie.Common.RawModels;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests; 

[TestClass]
public class StashTabAnalyzerTests : TestBase {
	[TestMethod]
	public async Task TestAnalyze_NoItems() {
		this._itemAnalyzer.SetupGet(c => c.Order).Returns(10);
		var tab = new RawStashTab("foo", false, null, null, null, "", "Scourge", Array.Empty<Item>());
		var tabAnalyzer = new StashTabAnalyzer(new[] {this._itemAnalyzer.Object});

		var analyzed = await tabAnalyzer.AnalyzeTab(tab);
		Assert.AreEqual("foo", analyzed.StashTabId);
		Assert.AreEqual(0, analyzed.Items.Length);
	}
	
	[TestMethod]
	public async Task TestAnalyze_WithItems() {
		var rawItems = new[] {
			await ItemUtils.ReadTestItem("boots")
		};
		
		this._itemAnalyzer.SetupGet(c => c.Order).Returns(10);
		this._itemAnalyzer.Setup(c => c.AnalyzeItems(
			It.Is<AnalyzedItem[]>(c=>c.Length == 1 && c[0].RawItem == rawItems[0] && !c[0].Analysis.Properties.Any()))
		).Returns(ValueTask.CompletedTask);

		
		var rawTab = new RawStashTab("foo", true, "account", "char", "stash", "standard", "Scourge", rawItems);
		var tabAnalyzer = new StashTabAnalyzer(new[] {this._itemAnalyzer.Object});

		var analyzed = await tabAnalyzer.AnalyzeTab(rawTab);
		
		Assert.AreEqual("foo", analyzed.StashTabId);
		Assert.AreEqual(1, analyzed.Items.Length);
		
		this._itemAnalyzer.VerifyAll();
	}

	private Mock<IItemAnalyzer> _itemAnalyzer = null!;
}