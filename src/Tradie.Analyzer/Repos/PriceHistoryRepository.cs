using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System;
using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer.Repos;

/// <summary>
/// Repository for storing price histories for items as they change over time.
/// </summary>
public interface IPriceHistoryRepository : IAsyncDisposable, IDisposable {
	/// <summary>
	/// Loads the previous price history entries for a set of item ids.
	/// </summary>
	Task<IEnumerable<ItemPriceHistory>> LoadLatestPricesForItems(IEnumerable<string> itemIds, CancellationToken cancellation);
	/// <summary>
	/// Loads the history of prices for a given item.
	/// </summary>
	Task<IEnumerable<ItemPriceHistory>> LoadPriceHistoryForItem(string itemId, CancellationToken cancellation);
	/// <summary>
	/// Adds a new record for the given items' price histories.
	/// </summary>
	Task RecordPriceHistories(IEnumerable<ItemPriceHistory> priceHistory, CancellationToken cancellation);
}

public class PriceHistoryRepository : IPriceHistoryRepository {
	public PriceHistoryRepository(AnalysisContext context) {
		this._context = context;
	}

	public async Task<IEnumerable<ItemPriceHistory>> LoadLatestPricesForItems(IEnumerable<string> itemIds, CancellationToken cancellation) {
		/*var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellation);
		await using var comm = new NpgsqlCommand(@"
			SELECT DISTINCT ""ItemId"", ""RecordedTime"", first_value(""Currency"") OVER w, first_value(""Amount"") OVER w, first_value(""Kind"") OVER w
			FROM ""ItemPriceHistory""
			WHERE ""ItemId"" = ANY(@itemIds)
			HAVING ""RecordedTime"" = first_value(""RecordedTime"")
			WINDOW w AS (PARTITION BY ""ItemId"" ORDER BY ""RecordedTime"" DESC)
		", conn);*/
		/*await using var comm = new NpgsqlCommand(@"
			SELECT DISTINCT ""ItemId"", ""RecordedTime"", first_value(""Currency"") OVER w, first_value(""Amount"") OVER w, first_value(""Kind"") OVER w
			FROM ""ItemPriceHistory""
			WHERE ""ItemId"" = ANY(@itemIds)
			--HAVING ""RecordedTime"" = first_value(""RecordedTime"") OVER w
			WINDOW w AS (PARTITION BY ""ItemId"" ORDER BY ""RecordedTime"" DESC)
		", conn);
		// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
		comm.Parameters.AddWithValue("itemIds", NpgsqlDbType.Array | NpgsqlDbType.Text, itemIds);
		
		await using var reader = await comm.ExecuteReaderAsync(cancellation);
		var result = new List<ItemPriceHistory>();
		while (await reader.ReadAsync(cancellation)) {
			result.Add(new ItemPriceHistory(
				reader.GetString(0),
				reader.GetDateTime(1),
				(Currency)reader.GetInt16(2),
				reader.GetFloat(3),
				(BuyoutKind)reader.GetInt16(4)
			));
		}*/
		
		var result = await this._context.ItemPriceHistories.FromSqlRaw(@"
			SELECT DISTINCT ON(""ItemId"") *
			FROM ""ItemPriceHistory""
			WHERE ""ItemId"" = ANY({0})
			ORDER BY ""ItemId"", ""RecordedTime"" DESC
		", new object[] { itemIds.ToArray() }).ToArrayAsync(cancellation);
		
		return result;
	}

	public async Task<IEnumerable<ItemPriceHistory>> LoadPriceHistoryForItem(string itemId, CancellationToken cancellation) {
		var res = await this._context.ItemPriceHistories
			.Where(x => x.ItemId == itemId)
			.OrderBy(c=>c.RecordedTime)
			.ToListAsync(cancellation);
		return res;
	}

	public async Task RecordPriceHistories(IEnumerable<ItemPriceHistory> priceHistory, CancellationToken cancellation) {
		await this._context.ItemPriceHistories.AddRangeAsync(priceHistory, cancellation);
		await this._context.SaveChangesAsync(cancellation);
	}

	public void Dispose() {
		this._context.Dispose();
	}

	public ValueTask DisposeAsync() {
		return this._context.DisposeAsync();
	}

	private readonly AnalysisContext _context;
}