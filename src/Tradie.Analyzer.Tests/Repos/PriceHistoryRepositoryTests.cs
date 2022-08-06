using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.TestUtils;
namespace Tradie.Analyzer.Tests.Repos;

[TestClass]
public class PriceHistoryRepositoryTests : TestBase {
	protected override void Initialize() {
		this._repo = new(this._context);
	}
	
	[TestMethod]
	public async Task TestLoadPriceHistoryForItem_Multiple() {
		await this.AddTestData();

		var histories = (await this._repo.LoadPriceHistoryForItem("1", CancellationToken.None))
			.ToArray();
		Assert.AreEqual(2, histories.Length);

		histories.ShouldDeepEqual(new ItemPriceHistory[] {
			new("1", new(Currency.Chaos, 2.4f, BuyoutKind.Fixed), DateTime.Parse("2022-01-01")),
			new("1", new(Currency.Chaos, 4f, BuyoutKind.Fixed), DateTime.Parse("2022-03-01"))
		});
	}
	
	[TestMethod]
	public async Task TestLoadPriceHistoryForItem_Single() {
		await this.AddTestData();

		var histories = (await this._repo.LoadPriceHistoryForItem("2", CancellationToken.None))
			.ToArray();
		Assert.AreEqual(1, histories.Length);
		
		histories.ShouldDeepEqual(new ItemPriceHistory[] {
			new("2", new(Currency.Exalted, 1.6f, BuyoutKind.Offer), DateTime.Parse("2022-01-01"))
		});
	}
	
	[TestMethod]
	public async Task TestLoadPriceHistoryForItem_None() {
		await this.AddTestData();

		var histories = (await this._repo.LoadPriceHistoryForItem("unused", CancellationToken.None))
			.ToArray();
		Assert.AreEqual(0, histories.Length);
	}

	[TestMethod]
	public async Task TestLoadLatestPricesForItems() {
		await this.AddTestData();
		
		var prices = (await this._repo.LoadLatestPricesForItems(new[] { "1", "2" }, CancellationToken.None))
			.ToArray();
		Assert.AreEqual(2, prices.Length);
		
		prices.ShouldDeepEqual(new ItemPriceHistory[] {
			new("1", new(Currency.Chaos, 4f, BuyoutKind.Fixed), DateTime.Parse("2022-03-01")),
			new("2", new(Currency.Exalted, 1.6f, BuyoutKind.Offer), DateTime.Parse("2022-01-01"))
		});
	}
	
	[TestMethod]
	public async Task TestLoadLatestPricesForItems_None() {
		await this.AddTestData();
		
		var prices = (await this._repo.LoadLatestPricesForItems(new[] { "unused" }, CancellationToken.None))
			.ToArray();
		Assert.AreEqual(0, prices.Length);
	}

	[TestMethod]
	public async Task RecordPriceHistories() {
		var histories = new ItemPriceHistory[] {
			new("1", new(Currency.Chaos, 2.4f, BuyoutKind.Fixed), DateTime.Parse("2022-01-01")),
			new("1", new(Currency.Chaos, 4f, BuyoutKind.Fixed), DateTime.Parse("2022-03-01")),
			new("2", new(Currency.Exalted, 1.6f, BuyoutKind.Fixed), DateTime.Parse("2022-01-01"))
		};
		
		await this._repo.RecordPriceHistories(histories, CancellationToken.None);

		var dbHistories = await this._context.ItemPriceHistories.ToArrayAsync();
		Assert.AreEqual(3, dbHistories.Length);
		
		dbHistories.ShouldDeepEqual(histories);
	}

	private async Task AddTestData() {
		var dt1 = DateTime.Parse("2022-01-01");
		var dt2 = DateTime.Parse("2022-03-01");
		await this._context.ItemPriceHistories.AddRangeAsync(new ItemPriceHistory[] {
			new("1", dt1, Currency.Chaos, 2.4f, BuyoutKind.Fixed),
			new("2", dt1, Currency.Exalted, 1.6f, BuyoutKind.Offer),
			new("3", dt1, Currency.None, 0, BuyoutKind.None),
			new("1", dt2, Currency.Chaos, 4f, BuyoutKind.Fixed)
		});
		await this._context.SaveChangesAsync();
	}

	private AnalysisContext _context = null!;
	private PriceHistoryRepository _repo = null!;
}