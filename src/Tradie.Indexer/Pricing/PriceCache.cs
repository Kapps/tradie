using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Tradie.Analyzer.Models;
using Tradie.Common;

namespace Tradie.Indexer.Pricing;

/// <summary>
/// A cache for storing currency pricing equivalencies.
/// </summary>
public interface IPriceCache {
	/// <summary>
	/// Loads the cached values from this cache.
	/// Returns null if no cached values are available.
	/// </summary>
	Task<IEnumerable<CurrencyPrice>?> LoadCachedValues(CancellationToken cancellationToken);

	/// <summary>
	/// Updates the cache with the given values.
	/// </summary>
	Task UpdateCachedValues(IEnumerable<CurrencyPrice> values, CancellationToken token);
}

/// <summary>
/// Implementation of IPriceCache to store cached values in S3 for 24 hours.
/// </summary>
public class S3PriceCache : IPriceCache {
	public S3PriceCache(IAmazonS3 s3Client, ILogger<S3PriceCache> logger) {
		this._s3Client = s3Client;
		this._logger = logger;
	}
	
	public async Task<IEnumerable<CurrencyPrice>?> LoadCachedValues(CancellationToken cancellationToken) {
		var req = new GetObjectRequest() {
			Key = this.GetCacheFile(),
			BucketName = TradieConfig.StorageBucket!,
			ModifiedSinceDateUtc = DateTime.Now.AddHours(-24)
		};
		GetObjectResponse resp;
		try {
			resp = await this._s3Client.GetObjectAsync(req, cancellationToken);
		} catch(Exception ex) {
			this._logger.LogWarning("Failed to get S3 cached contents: {Exception}", ex);
			return null;
		}

		await using var respStream = resp.ResponseStream;
		var prices = JsonSerializer.Deserialize<CurrencyPrice[]>(respStream);
		return prices;
	}

	public async Task UpdateCachedValues(IEnumerable<CurrencyPrice> values, CancellationToken cancellationToken) {
		await using var ms = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(values));
		
		var req = new PutObjectRequest() {
			Key = this.GetCacheFile(),
			BucketName = TradieConfig.StorageBucket,
			ContentType = "application/json",
			InputStream = ms
		};

		await this._s3Client.PutObjectAsync(req, cancellationToken);
	}

	private string GetCacheFile() {
		return $"{TradieConfig.League}_Currency.json";
	}
	
	private readonly IAmazonS3 _s3Client;
	private readonly ILogger<S3PriceCache> _logger;
}