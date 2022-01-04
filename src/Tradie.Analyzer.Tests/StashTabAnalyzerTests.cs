using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tradie.Analyzer.Tests.Analyzers;
using Tradie.Common.RawModels;
using Tradie.Common.Tests;

namespace Tradie.Analyzer.Tests; 

[TestClass]
public class StashTabAnalyzerTests {
	[TestMethod]
	public async Task TestAnalyze_NoItems() {
		var mockAnalyzer = Mock.Of<IItemAnalyzer>(MockBehavior.Strict);
		var tab = new RawStashTab("foo", false, null, null, null, "", "Scourge", Array.Empty<Item>());
		var tabAnalyzer = new StashTabAnalyzer(new[] {mockAnalyzer});

		var analyzed = await tabAnalyzer.AnalyzeTab(tab);
		Assert.AreEqual("foo", analyzed.StashTabId);
		Assert.AreEqual(0, analyzed.Items.Length);
	}
	
	[TestMethod]
	public async Task TestAnalyze_WithItems() {
		var rawItems = new[] {
			await ItemUtils.ReadTestItem("boots")
		};
		
		var mockAnalyzer = new Mock<IItemAnalyzer>(MockBehavior.Strict);
		mockAnalyzer.Setup(c => c.AnalyzeItems(
			It.Is<AnalyzedItem[]>(c=>c.Length == 1 && c[0].RawItem == rawItems[0] && !c[0].Analysis.Properties.Any()))
		).Returns(Task.CompletedTask);

		
		var rawTab = new RawStashTab("foo", true, "account", "char", "stash", "standard", "Scourge", rawItems);
		var tabAnalyzer = new StashTabAnalyzer(new[] {mockAnalyzer.Object});

		var analyzed = await tabAnalyzer.AnalyzeTab(rawTab);
		
		Assert.AreEqual("foo", analyzed.StashTabId);
		Assert.AreEqual(1, analyzed.Items.Length);
		
		mockAnalyzer.VerifyAll();
	}
}