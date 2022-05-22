using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer.Repos;

public interface IAffixRangeRepository : IAsyncDisposable {
	Task UpsertRanges(IEnumerable<AffixRange> ranges, CancellationToken cancellationToken);
}

public class AffixRangeRepository : IAffixRangeRepository {
	public AffixRangeRepository(AnalysisContext context) {
		this._context = context;
	}

	public ValueTask DisposeAsync() {
		throw new NotImplementedException();
	}

	public Task UpsertRanges(IEnumerable<AffixRange> ranges, CancellationToken cancellationToken) {
		throw new NotImplementedException();
	}

	private readonly AnalysisContext _context;
}