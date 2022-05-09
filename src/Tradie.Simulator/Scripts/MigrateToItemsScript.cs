using Npgsql;
using System.Collections.Concurrent;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.Simulator.Scripts;

public class MigrateToItemsScript : ScriptBase {
	public override async Task Run() {
		var context = new AnalysisContext();
		var conn = await context.GetOpenedConnection<NpgsqlConnection>();

		var tx = await conn.BeginTransactionAsync();

		await using var timeoutComm = new NpgsqlCommand("SET LOCAL statement_timeout TO 0", conn);
		await timeoutComm.ExecuteNonQueryAsync();

		Console.WriteLine("Creating temp table");
		await this.CreateHashTable(conn);
		
		Console.WriteLine("Loading hashes");
		var hashes = await LoadItemHashes(conn).ToArrayAsync();
		
		Console.WriteLine($"Generating {hashes.Length} hashes.");
		var partitioner = Partitioner.Create(0, hashes.Length);

		Parallel.ForEach(partitioner, range => {
			for(int i = range.Item1; i < range.Item2; i++) {
				ref var entry = ref hashes[i];
				ulong hash = LoggedItem.GenerateIdHash(entry.rawId);
				entry.hash = hash;
			}
		});
		
		Console.WriteLine("Beginning copy");
		var rows = await InsertHashes(conn, hashes);
		
		Console.WriteLine($"Inserting after writing {rows} rows.");
		var insertCount = await InsertIntoItems(conn);
		
		Console.WriteLine($"Committing after writing {insertCount} rows.");
		await tx.CommitAsync();
	}

	private async Task<int> InsertIntoItems(NpgsqlConnection conn) {
		await using var comm = new NpgsqlCommand(@"
			WITH elements AS (SELECT ""Id"", jsonb_array_elements(""Items"") item FROM ""StashTabs Clone"")
			INSERT INTO ""Items"" (""IdHash"", ""RawId"", ""Properties"", ""StashTabId"")
			SELECT DISTINCT ON(e.item ->> 'RawId') hashes.hash, e.item ->> 'RawId', e.item -> 'Properties', e.""Id""
			FROM elements e
			INNER JOIN hashes ON hashes.rawId = e.item ->> 'RawId'
		", conn);
		
		int count = await comm.ExecuteNonQueryAsync();
		return count;
	}

	private async Task<ulong> InsertHashes(NpgsqlConnection conn, HashMapping[] hashes) {
		await using var writer = await conn.BeginBinaryImportAsync("COPY hashes(rawId, hash) FROM STDIN (FORMAT BINARY)");

		foreach(var hash in hashes) {
			await writer.WriteRowAsync(CancellationToken.None, hash.rawId, (long)hash.hash);
		}

		ulong rows = await writer.CompleteAsync();
		return rows;
	}

	private async IAsyncEnumerable<HashMapping> LoadItemHashes(NpgsqlConnection conn) {
		await using var comm = new NpgsqlCommand(@"
			WITH elements AS (SELECT jsonb_array_elements(""Items"") item FROM ""StashTabs Clone"")
			SELECT DISTINCT ON(elements.item ->> 'RawId') elements.item ->> 'RawId' FROM elements", conn
		);

		await using var reader = await comm.ExecuteReaderAsync();
		while(await reader.ReadAsync()) {
			string rawId = reader.GetString(0);
			yield return new HashMapping(rawId, 0);
		}
	}

	private async Task CreateHashTable(NpgsqlConnection conn) {
		await using var comm = new NpgsqlCommand("CREATE TEMP TABLE hashes(rawId text PRIMARY KEY, hash bigint UNIQUE)", conn);
		await comm.ExecuteNonQueryAsync();
	}

	private record struct HashMapping(string rawId, ulong hash);
}