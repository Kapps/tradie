using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common.RawModels;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers;

[TestClass]
public class TradeAttributesAnalyzerTests : TestBase {
	[TestInitialize]
	public void Initializer() {
		this._analyzer = new TradePropertiesAnalyzer(this._priceHistoryRepo.Object);
	}
	
	[TestMethod]
	public async Task TestEmpty() {
		var items = Array.Empty<AnalyzedItem>();
		await this._analyzer.AnalyzeItems(items);
		Assert.AreEqual(0, items.Length);
	}

	[TestMethod]
	public async Task TestBasic_ExistingPrice() {
		var item = new AnalyzedItem(await ItemUtils.ReadTestItem("boots"));

		_priceHistoryRepo.Setup(c => c.LoadLatestPricesForItems(
			new[] {item.Id}.DeepMatcher<IEnumerable<string>>(),
			CancellationToken.None
		)).ReturnsAsync(new ItemPriceHistory[] {
			new(item.Id, new ItemPrice(Currency.Chaos, 40, BuyoutKind.Fixed), DateTime.Today.AddHours(-1))
		});
		
		_priceHistoryRepo.Setup(c => c.RecordPriceHistories(new List<ItemPriceHistory> {
			new(item.Id, new ItemPrice(Currency.Chaos, 75, BuyoutKind.Fixed), DateTime.Now)
		}.DeepMatcher(), CancellationToken.None)).Returns(Task.CompletedTask);
		
		await this._analyzer.AnalyzeItems(new[] { item });

		var props = (TradeListingAnalysis)item.Analysis[TradePropertiesAnalyzer.Id]!;
		Assert.IsNotNull(props);

		props.ShouldDeepEqual(new TradeListingAnalysis(
			8, 6,
			new ItemPrice(Currency.Chaos, 75, BuyoutKind.Fixed),
			"~price 75 chaos"
		));
	}
	
	[TestMethod]
	public async Task TestBasic_NoPreviousPrice() {
		var item = new AnalyzedItem(await ItemUtils.ReadTestItem("boots"));

		_priceHistoryRepo.Setup(c => c.LoadLatestPricesForItems(
			new[] {item.Id}.DeepMatcher<IEnumerable<string>>(),
			CancellationToken.None
		)).ReturnsAsync(new ItemPriceHistory[] {});
		
		_priceHistoryRepo.Setup(c => c.RecordPriceHistories(new List<ItemPriceHistory> {
			new(item.Id, new ItemPrice(Currency.Chaos, 75, BuyoutKind.Fixed), DateTime.Now)
		}.DeepMatcher(), CancellationToken.None)).Returns(Task.CompletedTask);
		
		await this._analyzer.AnalyzeItems(new[] { item });

		var props = (TradeListingAnalysis)item.Analysis[TradePropertiesAnalyzer.Id]!;
		Assert.IsNotNull(props);

		props.ShouldDeepEqual(new TradeListingAnalysis(
			8, 6,
			new ItemPrice(Currency.Chaos, 75, BuyoutKind.Fixed),
			"~price 75 chaos"
		));
	}

	[TestMethod]
	public async Task TestBasic_NoBuyout() {
		var item = new AnalyzedItem(await ItemUtils.ReadTestItem("nobuyout"));

		_priceHistoryRepo.Setup(c => c.LoadLatestPricesForItems(
			new[] { item.Id }.DeepMatcher<IEnumerable<string>>(),
			CancellationToken.None
		)).ReturnsAsync(new ItemPriceHistory[] {
			new(item.Id, new ItemPrice(Currency.Chaos, 40, BuyoutKind.Fixed), DateTime.Today.AddHours(-1))
		});

		_priceHistoryRepo.Setup(c => c.RecordPriceHistories(new List<ItemPriceHistory> {
			new(item.Id, ItemPrice.None, DateTime.Now)
		}.DeepMatcher(), CancellationToken.None)).Returns(Task.CompletedTask);

		await this._analyzer.AnalyzeItems(new[] { item });

		var props = (TradeListingAnalysis)item.Analysis[TradePropertiesAnalyzer.Id]!;
		Assert.IsNotNull(props);

		props.ShouldDeepEqual(new TradeListingAnalysis(
			8, 6,
			ItemPrice.None, 
			null
		));
	}
	
	[TestMethod]
	public async Task TestBasic_NoChange_NoPrice() {
		var item = new AnalyzedItem(await ItemUtils.ReadTestItem("nobuyout"));

		_priceHistoryRepo.Setup(c => c.LoadLatestPricesForItems(
			new[] { item.Id }.DeepMatcher<IEnumerable<string>>(),
			CancellationToken.None
		)).ReturnsAsync(new ItemPriceHistory[] {
			new(item.Id, ItemPrice.None, DateTime.Today.AddHours(-1))
		});

		await this._analyzer.AnalyzeItems(new[] { item });

		var props = (TradeListingAnalysis)item.Analysis[TradePropertiesAnalyzer.Id]!;
		Assert.IsNotNull(props);

		props.ShouldDeepEqual(new TradeListingAnalysis(
			8, 6,
			ItemPrice.None, 
			null
		));
	}
	
	[TestMethod]
	public async Task TestBasic_NoChange_WithPrice() {
		var item = new AnalyzedItem(await ItemUtils.ReadTestItem("boots"));

		_priceHistoryRepo.Setup(c => c.LoadLatestPricesForItems(
			new[] {item.Id}.DeepMatcher<IEnumerable<string>>(),
			CancellationToken.None
		)).ReturnsAsync(new ItemPriceHistory[] {
			new(item.Id, new ItemPrice(Currency.Chaos, 75, BuyoutKind.Fixed), DateTime.Today.AddHours(-1))
		});
		
		await this._analyzer.AnalyzeItems(new[] { item });

		var props = (TradeListingAnalysis)item.Analysis[TradePropertiesAnalyzer.Id]!;
		Assert.IsNotNull(props);

		props.ShouldDeepEqual(new TradeListingAnalysis(
			8, 6,
			new ItemPrice(Currency.Chaos, 75, BuyoutKind.Fixed),
			"~price 75 chaos"
		));
	}

	private TradePropertiesAnalyzer _analyzer = null!;
	private Mock<IPriceHistoryRepository> _priceHistoryRepo = null!;
}
