using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Runtime.CompilerServices;
using System.Transactions;
using Tradie.Analyzer;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.ItemLogBuilder.Postgres {
	/// <summary>
	/// A repository for ItemLog stash tabs.
	/// </summary>
	public interface ILoggedTabRepository : IAsyncDisposable {
		/// <summary>
		/// Logs the following tabs, returning the logged version of each tab.
		/// </summary>
		IAsyncEnumerable<LoggedStashTab> LogTabs(IAsyncEnumerable<AnalyzedStashTab> tabs, CancellationToken cancellationToken);
	}
	
	/// <summary>
	/// An implementation of IItemLogRepository that logs to a Postgres database context.
	/// </summary>
	public class PostgresLoggedTabRepository : ILoggedTabRepository {
		public PostgresLoggedTabRepository(AnalysisContext context) {
			this._context = context;
		}

		public async IAsyncEnumerable<LoggedStashTab> LogTabs(IAsyncEnumerable<AnalyzedStashTab> tabs, [EnumeratorCancellation] CancellationToken cancellationToken) {
			var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellationToken);

			Console.WriteLine("Creating temp table");
			await CreateTempTable(conn, cancellationToken);
			Console.WriteLine("Doing copy");
			await PerformCopy(conn, tabs, cancellationToken);

			Console.WriteLine("Upserting");
			var results = UpsertIntoPrimaryTable();

			Console.WriteLine("Yielding results");
			await foreach(var tab in results.WithCancellation(cancellationToken)) {
				//Console.WriteLine($"Returning tab {tab}");
				yield return tab;
			}

			Console.WriteLine("Done yielding");
		}

		
		public async ValueTask DisposeAsync() {
			await this._context.DisposeAsync();
		}
		

		private async Task CreateTempTable(NpgsqlConnection conn, CancellationToken cancellationToken) {
			await using var comm = new NpgsqlCommand($"DROP TABLE IF EXISTS {StashTempTableName};" +
			                                         $"CREATE TEMPORARY TABLE {StashTempTableName} (LIKE \"StashTabs\" INCLUDING IDENTITY) ON COMMIT DROP;", conn); 
			await comm.ExecuteNonQueryAsync(cancellationToken);
		}

		private async Task PerformCopy(NpgsqlConnection conn, IAsyncEnumerable<AnalyzedStashTab> tabs, CancellationToken cancellationToken) {
			await using var writer = await conn.BeginBinaryImportAsync($@"
				COPY {StashTempTableName} (""RawId"", ""Owner"", ""LastCharacterName"", ""Name"", ""League"", ""Created"", ""LastModified"")
				FROM STDIN (FORMAT BINARY);
			", cancellationToken);

			await foreach(var tab in tabs.WithCancellation(cancellationToken)) {
				await writer.StartRowAsync(cancellationToken);

				await writer.WriteAsync(tab.StashTabId, NpgsqlDbType.Text, cancellationToken);
				await writer.WriteAsync(tab.AccountName, NpgsqlDbType.Text, cancellationToken);
				await writer.WriteAsync(tab.LastCharacterName, NpgsqlDbType.Text, cancellationToken);
				await writer.WriteAsync(tab.Name, NpgsqlDbType.Text, cancellationToken);
				await writer.WriteAsync(tab.League, NpgsqlDbType.Text, cancellationToken);
				await writer.WriteAsync(DateTime.Now, NpgsqlDbType.Timestamp, cancellationToken);
				await writer.WriteAsync(DateTime.Now, NpgsqlDbType.Timestamp, cancellationToken);
			}

			await writer.CompleteAsync(cancellationToken);
		}

		private IAsyncEnumerable<LoggedStashTab> UpsertIntoPrimaryTable() {
			string query = $@"
				INSERT INTO ""StashTabs"" (""RawId"", ""Owner"", ""LastCharacterName"", ""Name"", ""League"", ""Created"", ""LastModified"")
					SELECT ""RawId"", ""Owner"", ""LastCharacterName"", ""Name"", ""League"", ""Created"", ""LastModified""
					FROM {StashTempTableName}
				ON CONFLICT (""RawId"") DO UPDATE
					SET ""LastModified"" = CURRENT_TIMESTAMP, ""LastCharacterName"" = excluded.""LastCharacterName"",
					""Name"" = excluded.""Name""
				RETURNING *
    		";

			return this._context.LoggedStashTabs.FromSqlRaw(query)
				.AsNoTracking()
				.AsAsyncEnumerable();
		}

		private const string StashTempTableName = "tmp_stash_tabs";
		private readonly AnalysisContext _context;
	}
}