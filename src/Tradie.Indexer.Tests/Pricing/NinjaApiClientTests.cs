using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tradie.Indexer.Pricing;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Pricing;

[TestClass]
public class NinjaApiClientTests : TestBase {
	protected override void Initialize() {
		this._client = new NinjaApiClient(TestUtils.TestUtils.CreateLogger<NinjaApiClient>());
	}

	[TestMethod]
	[Ignore("Requires refactoring to be testable.")]
	public Task TestGetPriceListings() {
		return Task.CompletedTask;
		// TODO: Refactor to take in an IApiClient; not testable as is.
	}

	private NinjaApiClient _client = null!;
}