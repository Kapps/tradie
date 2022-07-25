using Tradie.Common.Metrics;

namespace Tradie.Indexer;

public static class IndexerMetrics {
	public static readonly CustomMetric SearchTime = new CustomMetric(
		"Search Time (MS)",
		TimeSpan.FromMinutes(1)
	);

	public static readonly CustomMetric BlocksSearched = new(
		"Blocks Searched",
		TimeSpan.FromMinutes(1)
	);
}