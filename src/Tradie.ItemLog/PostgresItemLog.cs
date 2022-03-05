using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer;
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
			SELECT ""Id"", ""RawId"", ""Name"",  ""LastCharacterName"", ""Owner"", ""League"", ""Kind"", ""Items""
			FROM ""StashTabs""
			WHERE ""Id"" > $1
		", conn) {
			Parameters = {
				new NpgsqlParameter {Value = previousId, NpgsqlDbType = NpgsqlDbType.Bigint}
			}
		};

		await using var reader = await comm.ExecuteReaderAsync(cancellationToken);
		while(await reader.ReadAsync(cancellationToken)) {
			string itemsJson = reader.GetString(7);
			LoggedItem[] items = JsonSerializer.Deserialize<LoggedItem[]>(itemsJson, new JsonSerializerOptions(new JsonSerializerOptions() {
				Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
			}))!;

			yield return new LogRecord(
				new ItemLogOffset(reader.GetInt64(0).ToString()),
				new AnalyzedStashTab(
					reader.GetString(1),
					reader.IsDBNull(2) ? null : reader.GetString(2),
					reader.IsDBNull(3) ? null : reader.GetString(3),
					reader.IsDBNull(4) ? null : reader.GetString(4),
					reader.IsDBNull(5) ? null : reader.GetString(5),
					reader.IsDBNull(6) ? null : reader.GetString(6),
					items.Select(c=>new ItemAnalysis(c.RawId, c.Properties)).ToArray()
				)
			);
		}
	}

	private readonly AnalysisContext _context;
}
