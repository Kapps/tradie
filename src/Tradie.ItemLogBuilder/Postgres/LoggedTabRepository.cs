using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;
using Serializer=SpanJson.JsonSerializer.Generic.Utf8;

namespace Tradie.ItemLogBuilder.Postgres;

/// <summary>
/// A repository for ItemLog stash tabs.
/// </summary>
public interface ILoggedTabRepository : IAsyncDisposable {
	/// <summary>
	/// Logs the following tabs, returning the ID of the logged version of each tab.
	/// </summary>
	IAsyncEnumerable<TabMapping> LogTabs(IAsyncEnumerable<AnalyzedStashTab> tabs, CancellationToken cancellationToken);
}

/// <summary>
/// An implementation of IItemLogRepository that logs to a Postgres database context.
/// </summary>
public class PostgresLoggedTabRepository : ILoggedTabRepository {
	public PostgresLoggedTabRepository(AnalysisContext context, ILogger<PostgresLoggedTabRepository> logger) {
		this._context = context;
		this._logger = logger;
	}

	public async IAsyncEnumerable<TabMapping> LogTabs(IAsyncEnumerable<AnalyzedStashTab> tabs,
		[EnumeratorCancellation] CancellationToken cancellationToken) {
		var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellationToken);

		this._logger.LogDebug("Creating stash temp table");
		await CreateTempTable(conn, cancellationToken);
		_logger.LogDebug("Doing copy");
		await PerformCopy(conn, tabs, cancellationToken);

		_logger.LogDebug("Upserting");
		var results = UpsertIntoPrimaryTable(conn, cancellationToken);

		_logger.LogDebug("Yielding results");
		await foreach(var tabId in results.WithCancellation(cancellationToken)) {
			yield return tabId;
		}

		_logger.LogDebug("Done yielding");
	}


	public async ValueTask DisposeAsync() {
		await this._context.DisposeAsync();
	}


	private async Task CreateTempTable(NpgsqlConnection conn, CancellationToken cancellationToken) {
		await using var comm = new NpgsqlCommand($"DROP TABLE IF EXISTS {StashTempTableName};" +
		                                         $"CREATE TEMPORARY TABLE {StashTempTableName} (LIKE \"StashTabs\" INCLUDING IDENTITY) ON COMMIT DROP;",
			conn);
		await comm.ExecuteNonQueryAsync(cancellationToken);
	}

	private async Task PerformCopy(NpgsqlConnection conn, IAsyncEnumerable<AnalyzedStashTab> tabs,
		CancellationToken cancellationToken) {
		await using var writer = await conn.BeginBinaryImportAsync($@"
				COPY {StashTempTableName} (""RawId"", ""Owner"", ""LastCharacterName"", ""Name"", ""League"", ""Kind"", ""Created"", ""LastModified"", ""PackedItems"")
				FROM STDIN (FORMAT BINARY);
			", cancellationToken);

		_logger.LogDebug("Starting to write rows");

		await foreach(var tab in tabs.WithCancellation(cancellationToken)) {
			await writer.StartRowAsync(cancellationToken);

			await writer.WriteAsync(tab.StashTabId, NpgsqlDbType.Text, cancellationToken);
			await writer.WriteAsync(tab.AccountName, NpgsqlDbType.Text, cancellationToken);
			await writer.WriteAsync(tab.LastCharacterName, NpgsqlDbType.Text, cancellationToken);
			await writer.WriteAsync(tab.Name, NpgsqlDbType.Text, cancellationToken);
			await writer.WriteAsync(tab.League, NpgsqlDbType.Text, cancellationToken);
			await writer.WriteAsync(tab.Kind, NpgsqlDbType.Text, cancellationToken);
			await writer.WriteAsync(DateTime.Now, NpgsqlDbType.Timestamp, cancellationToken);
			await writer.WriteAsync(DateTime.Now, NpgsqlDbType.Timestamp, cancellationToken);
			await writer.WriteAsync(SerializeItemsPacked(tab.Items), NpgsqlDbType.Bytea, cancellationToken);
		}

		_logger.LogDebug("Finished writing rows");

		await writer.CompleteAsync(cancellationToken);

		this._logger.LogDebug("Finished completing copy");
	}

	private byte[] SerializeItemsPacked(ItemAnalysis[] items) {
		return MessagePack.MessagePackSerializer.Serialize(items, MessagePackedStashTabSerializer.SerializationOptions);
	}

	private async IAsyncEnumerable<TabMapping> UpsertIntoPrimaryTable(NpgsqlConnection conn,
		[EnumeratorCancellation] CancellationToken cancellationToken) {
		string query = $@"
				INSERT INTO ""StashTabs"" (""RawId"", ""Owner"", ""LastCharacterName"", ""Name"", ""League"", ""Kind"", ""Created"", ""LastModified"", ""PackedItems"")
					SELECT ""RawId"", ""Owner"", ""LastCharacterName"", ""Name"", ""League"", ""Kind"", ""Created"", ""LastModified"", ""PackedItems""
					FROM {StashTempTableName}
				ON CONFLICT (""RawId"") DO UPDATE
					SET ""LastModified"" = CURRENT_TIMESTAMP, ""LastCharacterName"" = excluded.""LastCharacterName"",
					""Name"" = excluded.""Name"", ""PackedItems"" = excluded.""PackedItems""
				RETURNING ""Id"", ""RawId""
    		";

		this._logger.LogDebug("Executing reader");

		var comm = new NpgsqlCommand(query, conn);
		await using var reader = await comm.ExecuteReaderAsync(cancellationToken);

		this._logger.LogDebug("Starting to read");

		while(await reader.ReadAsync(cancellationToken)) {
			long id = reader.GetInt64(0);
			string rawId = reader.GetString(1);
			yield return new TabMapping(rawId, id);
		}

		this._logger.LogDebug("Done reading");
	}

	private const string StashTempTableName = "tmp_stash_tabs";
	private readonly AnalysisContext _context;
	private readonly ILogger<ILoggedTabRepository> _logger;
}

/// <summary>
/// Maps a raw stash tab ID to its inserted database id.
/// </summary>
public readonly record struct TabMapping(string RawStashTabId, long StashTabId);