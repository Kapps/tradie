using Npgsql;
using NpgsqlTypes;
using RestSharp.Extensions;
using Tradie.Analyzer;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;

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
	
	public async IAsyncEnumerable<LogRecord> GetItems(ItemLogOffset offset, CancellationToken cancellationToken) {
		long previousId = long.Parse(offset.Offset ?? "0");

		var conn = await this._context.GetOpenedConnection<NpgsqlConnection>(cancellationToken);

		var comm = new NpgsqlCommand(@"
			SELECT ""Id"", ""RawId"", ""Name"",  ""LastCharacterName"", ""Owner"", ""League"", ""Kind"", ""Items""
			FROM ""StashTabs""
			WHERE ""Id"" > $1
		", conn) {
			Parameters = {
				new() {Value = previousId, NpgsqlDbType = NpgsqlDbType.Bigint}
			}
		};

		await using var reader = await comm.ExecuteReaderAsync(cancellationToken);
		while(await reader.ReadAsync(cancellationToken)) {
			string itemsJson = reader.GetString(7);
			ItemAnalysis[] items = SpanJson.JsonSerializer.Generic.Utf16.Deserialize<ItemAnalysis[]>(itemsJson);

			yield return new LogRecord(
				new ItemLogOffset(reader.GetInt64(0).ToString()),
				new AnalyzedStashTab(
					reader.GetString(1),
					reader.IsDBNull(2) ? null : reader.GetString(2),
					reader.IsDBNull(3) ? null : reader.GetString(3),
					reader.IsDBNull(4) ? null : reader.GetString(4),
					reader.IsDBNull(5) ? null : reader.GetString(5),
					reader.IsDBNull(6) ? null : reader.GetString(6),
					items
				)
			);
		}
	}

	private readonly AnalysisContext _context;
}
