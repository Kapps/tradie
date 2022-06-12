using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tradie.Analyzer.Entities;
using Tradie.Common;

namespace Tradie.Analyzer.Repos;

public interface IAffixRangeRepository : IAsyncDisposable, IDisposable {
	Task UpsertRanges(IEnumerable<AffixRange> ranges, CancellationToken cancellationToken);
}

public class AffixRangeRepository : IAffixRangeRepository {
	private const string TempTableName = "_temp_affixes";
	private const string PrimaryTableName = "AffixRanges";

	public AffixRangeRepository(AnalysisContext context) {
		this._context = context;
	}

	public async ValueTask DisposeAsync() {
		await this._context.DisposeAsync();
	}
	
	void IDisposable.Dispose() {
		this._context.Dispose();
	}

	public async Task UpsertRanges(IEnumerable<AffixRange> ranges, CancellationToken cancellationToken) {
		var conn = await this._context.GetOpenedConnection<NpgsqlConnection>();

		await CreateTempTable(conn, cancellationToken);
		await InsertIntoTempTable(ranges, cancellationToken);
		await UpsertIntoPrimaryTable(conn, cancellationToken);
	}

	private async Task CreateTempTable(NpgsqlConnection connection, CancellationToken cancellationToken) {
		await using var comm = new NpgsqlCommand($@"
			CREATE TEMPORARY TABLE ""{TempTableName}""
				(LIKE ""{PrimaryTableName}"" INCLUDING IDENTITY)
				ON COMMIT DROP;
		", connection);

		await comm.ExecuteNonQueryAsync(cancellationToken);
	}

	private async Task InsertIntoTempTable(IEnumerable<AffixRange> ranges, CancellationToken cancellationToken) {
		await _context.BulkInsertAsync(ranges.ToList(), new BulkConfig() {
			CustomDestinationTableName = TempTableName
		}, cancellationToken: cancellationToken);
	}

	private async Task UpsertIntoPrimaryTable(NpgsqlConnection connection, CancellationToken cancellationToken) {
		await using var comm = new NpgsqlCommand($@"
			INSERT INTO ""AffixRanges""
				SELECT * FROM ""{TempTableName}"" t
				WHERE NOT EXISTS(
					SELECT * FROM ""AffixRanges"" r
					WHERE
						r.""ModHash"" = t.""ModHash""
						AND r.""EntityKind"" = t.""EntityKind""
						AND r.""ModCategory"" = t.""ModCategory""
						AND (r.""MinValue"" IS NOT NULL AND r.""MinValue"" <= t.""MinValue"")
						AND (r.""MaxValue"" IS NOT NULL AND r.""MaxValue"" >= t.""MaxValue"")
				)
				ORDER BY t.""ModHash"", t.""EntityKind"", t.""ModCategory""
			ON CONFLICT(""ModHash"", ""EntityKind"", ""ModCategory"")
			DO UPDATE SET
				""MinValue"" = LEAST(""AffixRanges"".""MinValue"", excluded.""MinValue""),
				""MaxValue"" = GREATEST(""AffixRanges"".""MaxValue"", excluded.""MaxValue"")
			WHERE
				(""AffixRanges"".""MinValue"" IS NULL OR excluded.""MinValue"" < ""AffixRanges"".""MinValue"")
				OR (""AffixRanges"".""MaxValue"" IS NULL OR excluded.""MaxValue"" > ""AffixRanges"".""MaxValue"")
		", connection);

		await comm.ExecuteNonQueryAsync(cancellationToken);
	}

	private readonly AnalysisContext _context;
}