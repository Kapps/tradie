using MessagePack;
using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Entities;
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

		var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellationToken);

		var comm = new NpgsqlCommand(@"
			SELECT ""PackedItems"", ""Id"", ""RawId"", ""Name"",  ""LastCharacterName"", ""Owner"", ""League"", ""Kind""
			FROM ""StashTabs""
			WHERE ""Id"" > $1
		", conn) {
			Parameters = {
				new NpgsqlParameter {Value = previousId, NpgsqlDbType = NpgsqlDbType.Bigint}
			}
		};

		await using var reader = await comm.ExecuteReaderAsync(cancellationToken);
		while(await reader.ReadAsync(cancellationToken)) {
			// ReSharper disable once UseAwaitUsing
			// ReSharper disable once MethodHasAsyncOverloadWithCancellation
			using var dataStream = reader.IsDBNull(0) ? null : reader.GetStream(0);
			var items = dataStream == null ? Array.Empty<ItemAnalysis>() : MessagePackSerializer.Deserialize<ItemAnalysis[]>(dataStream,
				MessagePackedStashTabSerializer.SerializationOptions, cancellationToken);
			
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
		}
	}

	private readonly AnalysisContext _context;
}
