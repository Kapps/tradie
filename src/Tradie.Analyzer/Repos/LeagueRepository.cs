using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using SpanJson;
using Tradie.Common;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Repos;

/// <summary>
/// Repository for accessing information about challenge or permanent leagues.
/// </summary>
public interface ILeagueRepository : IAsyncDisposable {
	/// <summary>
	/// Returns all available leagues.
	/// </summary>
	Task<IEnumerable<League>> GetAll(CancellationToken cancellationToken);
}

public class LeagueRepository : ILeagueRepository {
	private const string LeagueCacheKey = "Leagues";

	
	public LeagueRepository(IDistributedCache cache, AnalysisContext context) {
		this._cache = cache;
		this._context = context;
	}
	
	public async Task<IEnumerable<League>> GetAll(CancellationToken cancellationToken) {
		var cachedLeagues = await this._cache.GetAsync(LeagueCacheKey, cancellationToken);
		if(cachedLeagues != null) {
			return SpanJson.JsonSerializer.Generic.Utf8.Deserialize<League[]>(cachedLeagues);
		}

		var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellationToken);
		await using var comm = new NpgsqlCommand(@"
			SELECT ""League"", COUNT(*) FROM ""StashTabs""
			WHERE ""League"" IS NOT NULL AND ""LastModified"" > (CURRENT_TIMESTAMP - INTERVAL '70 days')
			GROUP BY ""League""
			HAVING COUNT(*) > 100
			ORDER BY 2 DESC
		", conn);

		await using var reader = await comm.ExecuteReaderAsync(cancellationToken);
		var results = new List<League>();
		while(await reader.ReadAsync(cancellationToken)) {
			string id = reader.GetString(0);
			results.Add(new League(id));
		}

		await this._cache.SetAsync(LeagueCacheKey, JsonSerializer.Generic.Utf8.Serialize(results),
			new DistributedCacheEntryOptions() {
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
			}, cancellationToken);

		return results;
	} 
	
	public ValueTask DisposeAsync() {
		return this._context.DisposeAsync();
	}
	
	private readonly IDistributedCache _cache;
	private readonly AnalysisContext _context;
}