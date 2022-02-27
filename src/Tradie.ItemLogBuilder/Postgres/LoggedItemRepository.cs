using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;

#if false

namespace Tradie.ItemLogBuilder.Postgres {
	/// <summary>
	/// A repository for ItemLog stash tabs.
	/// </summary>
	public interface ILoggedItemRepository : IAsyncDisposable {
		/// <summary>
		/// Logs the following analyzed items, returning the saved version of each item. 
		/// </summary>
		IAsyncEnumerable<LoggedItem> LogItems(IAsyncEnumerable<LoggedItem> items, CancellationToken cancellationToken);
	}
	
	/// <summary>
	/// LoggedItemRepository implementation to store items as primarily JSON objects within a Postgres database.
	/// </summary>
	public class PostgresLoggedItemRepository : ILoggedItemRepository {
		public PostgresLoggedItemRepository(AnalysisContext context) {
			this._context = context;
		}

		public async IAsyncEnumerable<LoggedItem> LogItems(IAsyncEnumerable<LoggedItem> items, [EnumeratorCancellation] CancellationToken cancellationToken) {
			var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellationToken);
			
			//string tempTableName = $"tmp_items_{Guid.NewGuid():N}";
			string tempTableName = "tmp_items";

			Console.WriteLine("Creating item temp table");
			await CreateTempTable(conn, tempTableName, cancellationToken);
			Console.WriteLine("Starting copy");
			await PerformCopy(conn, tempTableName, items, cancellationToken);

			Console.WriteLine("Upserting to primary");
			var results = UpsertIntoPrimaryTable(tempTableName);

			Console.WriteLine("Yielding items");
			await foreach(var item in results.WithCancellation(cancellationToken)) {
				//`Console.WriteLine($"Returning item {item}");
				yield return item;
			}

			Console.WriteLine("Done yielding items.");
		}

		public async ValueTask DisposeAsync() {
			await this._context.DisposeAsync();
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
			Console.WriteLine("Beginning binary import.");
			await using var writer = await conn.BeginBinaryImportAsync($@"
				COPY {tempTableName} (""RawId"", ""StashTabId"", ""Properties"")
				FROM STDIN (FORMAT BINARY);
			", cancellationToken);

			await foreach(var item in items.WithCancellation(cancellationToken)) {
				await writer.StartRowAsync(cancellationToken);
				
				await writer.WriteAsync(item.RawId, NpgsqlDbType.Text, cancellationToken);
				await writer.WriteAsync(item.StashTabId, NpgsqlDbType.Bigint, cancellationToken);
				await writer.WriteAsync(item.Properties, NpgsqlDbType.Jsonb, cancellationToken);
			}

			Console.WriteLine("Completing async.");
			await writer.CompleteAsync(cancellationToken);
			Console.WriteLine("Completed");
		}

		private IAsyncEnumerable<LoggedItem> UpsertIntoPrimaryTable(string tempTableName) {
			string query = $@"
				INSERT INTO ""Items"" (""RawId"", ""StashTabId"", ""Properties"")
					SELECT ""RawId"", ""StashTabId"", ""Properties""
					FROM {tempTableName}
				-- ON CONFLICT (""RawId"") DO UPDATE
				ON CONFLICT DO NOTHING
					--SET ""StashTabId"" = excluded.""StashTabId"", ""Properties"" = excluded.""Properties""
				RETURNING *;
    		";

			return this._context.LoggedItems.FromSqlRaw(query)
				.AsNoTracking()
				.AsAsyncEnumerable();
		}
		
		private readonly AnalysisContext _context;
	}
}
#endif