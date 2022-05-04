using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using SpanJson;
using System.Runtime.CompilerServices;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.ItemLogBuilder.Postgres {
	/// <summary>
	/// A repository for ItemLog stash tabs.
	/// </summary>
	public interface ILoggedItemRepository : IAsyncDisposable {
		/// <summary>
		/// Logs the following analyzed items, returning the saved version of each item.
		/// Any items previously stored for the given stash tabs that are not present currently are deleted. 
		/// </summary>
		IAsyncEnumerable<LoggedItem> LogItems(
			TabMapping[] stashTabs,
			IAsyncEnumerable<LoggedItem> items,
			CancellationToken cancellationToken
		);
	}
	
	/// <summary>
	/// LoggedItemRepository implementation to store items as primarily JSON objects within a Postgres database.
	/// </summary>
	public class PostgresLoggedItemRepository : ILoggedItemRepository {
		public PostgresLoggedItemRepository(AnalysisContext context) {
			this._context = context;
		}

		public async IAsyncEnumerable<LoggedItem> LogItems(
			TabMapping[] stashTabs,
			IAsyncEnumerable<LoggedItem> items,
			[EnumeratorCancellation] CancellationToken cancellationToken
		) {
			var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellationToken);
			
			//string tempTableName = $"tmp_items_{Guid.NewGuid():N}";
			string tempTableName = "tmp_items";

			await CreateTempTable(conn, tempTableName, cancellationToken);
			await PerformCopy(conn, tempTableName, items, cancellationToken);

			var deletedCount = await DeleteFromPrimaryTable(conn, stashTabs, tempTableName)
				.CountAsync(cancellationToken);
			
			var results = UpsertIntoPrimaryTable(tempTableName);

			await foreach(var item in results.WithCancellation(cancellationToken)) {
				yield return item;
			}
		}

		public async ValueTask DisposeAsync() {
			await this._context.DisposeAsync();
		}

		private async IAsyncEnumerable<string> DeleteFromPrimaryTable(NpgsqlConnection conn, TabMapping[] stashTabs, string tempTableName) {
			string inClause = String.Join(", ", stashTabs.Select(c => c.StashTabId.ToString()));
			await using var comm = new NpgsqlCommand(
				$"DELETE FROM \"Items\"" +
				$"WHERE \"Items\".\"StashTabId\" = ANY(ARRAY[{inClause}]::int[])" +
				$"AND NOT EXISTS(SELECT * FROM {tempTableName} t WHERE t.\"IdHash\" = \"Items\".\"IdHash\")" +
				$"RETURNING \"RawId\"", conn
			);
			await using var reader = await comm.ExecuteReaderAsync();
			while(await reader.ReadAsync()) {
				string rawId = reader.GetString(0);
				yield return rawId;
			}
		}
		
		private async Task CreateTempTable(NpgsqlConnection conn, string tempTableName, CancellationToken cancellationToken) {
			await using var comm = new NpgsqlCommand($"DROP TABLE IF EXISTS {tempTableName};" +
			                                         $"CREATE TEMPORARY TABLE {tempTableName} (LIKE \"Items\" INCLUDING IDENTITY) ON COMMIT DROP;", conn); 
			await comm.ExecuteNonQueryAsync(cancellationToken);
		}

		private async Task PerformCopy(
			NpgsqlConnection conn,
			string tempTableName,
			IAsyncEnumerable<LoggedItem> items,
			CancellationToken cancellationToken
		) {
			await using var writer = await conn.BeginBinaryImportAsync($@"
				COPY {tempTableName} (""RawId"", ""IdHash"", ""StashTabId"", ""Properties"")
				FROM STDIN (FORMAT BINARY);
			", cancellationToken);

			await foreach(var item in items.WithCancellation(cancellationToken)) {
				await writer.StartRowAsync(cancellationToken);

				await writer.WriteAsync(item.RawId, NpgsqlDbType.Text, cancellationToken);
				await writer.WriteAsync((long)item.IdHash, NpgsqlDbType.Bigint, cancellationToken);
				await writer.WriteAsync(item.StashTabId, NpgsqlDbType.Bigint, cancellationToken);
				await writer.WriteAsync(item.Properties, NpgsqlDbType.Jsonb, cancellationToken);
			}

			await writer.CompleteAsync(cancellationToken);
		}

		private IAsyncEnumerable<LoggedItem> UpsertIntoPrimaryTable(string tempTableName) {
			string query = $@"
				INSERT INTO ""Items"" (""RawId"", ""IdHash"", ""StashTabId"", ""Properties"")
					SELECT ""RawId"", ""IdHash"", ""StashTabId"", ""Properties""
					FROM {tempTableName}
				ON CONFLICT (""IdHash"") DO UPDATE
					SET ""StashTabId"" = excluded.""StashTabId"", ""Properties"" = excluded.""Properties""
				RETURNING *
    		";

			return this._context.LoggedItems.FromSqlRaw(query)
				.AsNoTracking()
				.AsAsyncEnumerable();
		}
		
		private readonly AnalysisContext _context;
	}
}
