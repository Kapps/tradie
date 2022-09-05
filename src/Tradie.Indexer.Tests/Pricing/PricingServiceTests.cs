using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Models;
using Tradie.Indexer.Pricing;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Pricing;

[TestClass]
public class PricingServiceTests : TestBase {
	protected override void Initialize() {
		this._pricingService = new NinjaPricingService(this._priceCache.Object, this._ninjaApi.Object,
			TestUtils.TestUtils.CreateLogger<NinjaPricingService>());
		this._priceList = new CurrencyPrice[] {
			new(Currency.Alchemy, 0.5f),
			new(Currency.Alterations, 0.5f),
			new(Currency.Chaos, 1),
			new(Currency.Chromatics, 0.5f),
			new(Currency.Exalted, 120),
			new(Currency.Fuse, 0.5f),
			new(Currency.Gemcutters, 0.1f),
			new(Currency.Mirror, 60000),
			new(Currency.Vaal, 0.5f),
			new(Currency.Divine, 10)
		};
	}

	[TestMethod]
	public async Task TestLoadFromCache_AllAvailable() {
		this._priceCache.Setup(c => c.LoadCachedValues(CancellationToken.None))
			.ReturnsAsync(this._priceList);

		var resp = await this._pricingService.GetChaosEquivalentPrice(
			new ItemPrice(Currency.Exalted, 2.5f, BuyoutKind.Fixed),
			CancellationToken.None);

		Assert.AreEqual(300, resp);
	}
	
	[TestMethod]
	public async Task TestLoadFromCache_SomeUnavailable() {
		this._priceCache.Setup(c => c.LoadCachedValues(CancellationToken.None))
			.ReturnsAsync(this._priceList.Take(5).ToArray());

		await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
			this._pricingService.GetChaosEquivalentPrice(
				new ItemPrice(Currency.Exalted, 2.5f, BuyoutKind.Fixed),
				CancellationToken.None).AsTask());
	}

	private CurrencyPrice[] _priceList = null!;
	private Mock<IPriceCache> _priceCache = null!;
	private Mock<INinjaApi> _ninjaApi = null!;
	private NinjaPricingService _pricingService = null!;
}