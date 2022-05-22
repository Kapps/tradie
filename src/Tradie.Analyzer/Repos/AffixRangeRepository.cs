using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer.Repos;

public interface IAffixRangeRepository : IAsyncDisposable {
	Task UpsertRanges(IEnumerable<AffixRange> ranges, CancellationToken cancellationToken);
}

public class AffixRangeRepository {

}