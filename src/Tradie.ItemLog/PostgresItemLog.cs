using MessagePack;
using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.ItemLog;

/// <summary>
/// An ItemLog implementation that reads records from a Postgres database.
/// </summary>
public class PostgresItemLog : IItemLog, IAsyncDisposable {
	public PostgresItemLog(AnalysisContext context) {
		this._context = context;
	}

	public async ValueTask DisposeAsync() {
		await _context.DisposeAsync();
	}
	
	public async IAsyncEnumerable<LogRecord> GetItems(ItemLogOffset offset, [EnumeratorCancellation] CancellationToken cancellationToken) {
		long previousId = long.Parse(offset.Offset ?? "0");

		var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(CancellationToken.None);

		string query = @"
			SELECT ""PackedItems"", ""Id"", ""RawId"", ""Name"",  ""LastCharacterName"", ""Owner"", ""League"", ""Kind""
			FROM ""StashTabs""
			WHERE ""Id"" > $1 AND ""League"" = $2 AND ""Owner"" IS NOT NULL -- (aka, public tabs)
			ORDER BY ""Id"" DESC LIMIT 10000
		";
		var comm = new NpgsqlCommand(query, conn) {
			Parameters = {
				new NpgsqlParameter {Value = previousId, NpgsqlDbType = NpgsqlDbType.Bigint},
				new NpgsqlParameter {Value = (object?)TradieConfig.League ?? DBNull.Value, NpgsqlDbType = NpgsqlDbType.Text}
			}
		};

		await using var reader = await comm.ExecuteReaderAsync(CancellationToken.None);
		while(await reader.ReadAsync(CancellationToken.None)) {
			await using var dataStream = reader.IsDBNull(0) ? null : await reader.GetStreamAsync(0, CancellationToken.None);
			var items = dataStream == null ? Array.Empty<ItemAnalysis>() : MessagePackSerializer.Deserialize<ItemAnalysis[]>(dataStream,
				MessagePackedStashTabSerializer.SerializationOptions, CancellationToken.None);
			
			yield return new LogRecord(
				new ItemLogOffset(reader.GetInt64(1).ToString()),
				new AnalyzedStashTab(
					reader.GetString(2),
					reader.IsDBNull(3) ? null : reader.GetString(3),
					reader.IsDBNull(4) ? null : reader.GetString(4),
					reader.IsDBNull(5) ? null : reader.GetString(5),
					reader.IsDBNull(6) ? null : reader.GetString(6),
					reader.IsDBNull(7) ? null : reader.GetString(7),
					items
				)
			);
			
			if(cancellationToken.IsCancellationRequested) {
				break;
			}
		}
	}

	private readonly AnalysisContext _context;
}
