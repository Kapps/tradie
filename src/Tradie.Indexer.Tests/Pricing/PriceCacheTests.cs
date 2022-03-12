using Amazon.S3;
using Amazon.S3.Model;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Models;
using Tradie.Common;
using Tradie.Indexer.Pricing;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Pricing;

[TestClass]
public class PriceCacheTests : TestBase {
	protected override void Initialize() {
		this._priceCache = new S3PriceCache(this._s3Mock.Object, TestUtils.TestUtils.CreateLogger<S3PriceCache>());
	}

	[TestMethod]
	public async Task TestGetFromCache_Success() {
		string json = @"[
			{ ""Currency"": 2, ""ChaosEquivalentCost"": 144.3 },
			{ ""Currency"": 1, ""ChaosEquivalentCost"": 1 }
		]";

		this._s3Mock.Setup(c => c.GetObjectAsync(new GetObjectRequest() {
			Key = "Anarchy_Currency.json",
			BucketName = TradieConfig.StorageBucket,
			ModifiedSinceDateUtc = DateTime.Now.AddHours(-24)
		}.DeepMatcher(), CancellationToken.None)).ReturnsAsync(new GetObjectResponse() {
			ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes(json))
		});

		var resp = await this._priceCache.LoadCachedValues(CancellationToken.None);
		
		Assert.IsNotNull(resp);
		resp.ShouldDeepEqual(new[] {
			new CurrencyPrice(Currency.Exalted, 144.3f),
			new CurrencyPrice(Currency.Chaos, 1)
		});
	}

	[TestMethod]
	public async Task TestGetFromCache_Failed() {
		this._s3Mock.Setup(c => c.GetObjectAsync(new GetObjectRequest() {
			Key = "Anarchy_Currency.json",
			BucketName = TradieConfig.StorageBucket,
			ModifiedSinceDateUtc = DateTime.Now.AddHours(-24)
		}.DeepMatcher(), CancellationToken.None)).ThrowsAsync(new AmazonS3Exception("arr"));

		var resp = await this._priceCache.LoadCachedValues(CancellationToken.None);
		Assert.IsNull(resp);
	}

	[TestMethod]
	public async Task TestUpdateCache_Success() {
		var values = new CurrencyPrice[] {
			new CurrencyPrice(Currency.Exalted, 144.4f),
			new CurrencyPrice(Currency.Chaos, 1)
		};

		this._s3Mock.Setup(c => c.PutObjectAsync(new PutObjectRequest() {
			Key = "Anarchy_Currency.json",
			BucketName = TradieConfig.StorageBucket,
			ContentType = "application/json",
			InputStream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(values, (JsonSerializerOptions?)null))
		}.DeepMatcher(), CancellationToken.None)).ReturnsAsync(new PutObjectResponse());

		await this._priceCache.UpdateCachedValues(values, CancellationToken.None);
	}

	private S3PriceCache _priceCache = null!;
	private Mock<IAmazonS3> _s3Mock = null!;
}