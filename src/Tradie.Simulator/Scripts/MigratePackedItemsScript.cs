using MessagePack;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.Simulator.Scripts;

public class MigratePackedItemsScript : ScriptBase {
	public override async Task Run() {
		AnalysisContext ctx = new AnalysisContext();
		NpgsqlConnection conn = await ctx.GetOpenedConnection<NpgsqlConnection>();
		NpgsqlCommand comm =
			new NpgsqlCommand("SELECT \"Id\", \"Items\" FROM \"StashTabs\" WHERE \"PackedItems\" IS NULL AND \"Items\" <> '[]'", conn);

		int totalProcessed = 0;
		List<(long, byte[]?)> updates = new();
		await using NpgsqlDataReader reader = await comm.ExecuteReaderAsync();

		async Task SendUpdates() {
			await using AnalysisContext newContext = new AnalysisContext();
			NpgsqlConnection nextConn = await newContext.GetOpenedConnection<NpgsqlConnection>();

			NpgsqlCommand comm = new NpgsqlCommand(@$"
				UPDATE ""StashTabs""
				SET ""PackedItems"" = upd.""Items""
				FROM (VALUES
				    {string.Join(", ", Enumerable.Range(1, updates.Count).Select(c => $"(${(c * 2) - 1}, ${c * 2})"))}
				) AS upd(""Id"", ""Items"")
				WHERE ""StashTabs"".""Id"" = upd.""Id""; 
			", nextConn);

			foreach((long, byte[]?) update in updates) {
				comm.Parameters.Add(new NpgsqlParameter {
					Value = update.Item1,
					NpgsqlDbType = NpgsqlDbType.Bigint
				});
				comm.Parameters.Add(new NpgsqlParameter {
					Value = update.Item2 == null ? DBNull.Value : update.Item2,
					NpgsqlDbType = NpgsqlDbType.Bytea
				});
			}

			int rowsAffected = await comm.ExecuteNonQueryAsync();
			if(rowsAffected != updates.Count) {
				throw new DataException(
					$"Got {rowsAffected} rows affected instead of the expected {updates.Count}.");
			}

			updates.Clear();
		}

		while(await reader.ReadAsync()) {
			long id = reader.GetInt64(0);
			string json = reader.GetString(1);

			LoggedItem[] items = JsonSerializer.Deserialize<LoggedItem[]>(json, new JsonSerializerOptions {
				Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)}
			})!;

			ItemAnalysis[] converted = items.Select(c => new ItemAnalysis(c.RawId, c.Properties)).ToArray();
			byte[]? serialized = items.Length == 0
				? null
				: MessagePackSerializer.Serialize(converted, MessagePackedStashTabSerializer.SerializationOptions);
			updates.Add((id, serialized));

			if(updates.Count % 100 == 0) {
				await SendUpdates();
			}

			totalProcessed++;
			if(totalProcessed % 10000 == 0) {
				Console.WriteLine($"Processed {totalProcessed} items.");
			}
		}

		await SendUpdates();
	}
}