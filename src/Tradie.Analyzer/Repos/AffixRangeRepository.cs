using EFCore.BulkExtensions;
using Npgsql;
using Tradie.Analyzer.Entities;
using Tradie.Common;

namespace Tradie.Analyzer.Repos;

public interface IAffixRangeRepository : IAsyncDisposable {
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
				SELECT * FROM ""{TempTableName}""
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