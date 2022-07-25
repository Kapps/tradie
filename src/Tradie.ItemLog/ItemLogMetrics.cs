using Tradie.Common;
using Tradie.Common.Metrics;

namespace Tradie.ItemLog;

public static class ItemLogMetrics {
	public static readonly CustomMetric KinesisStreamLag = new CustomMetric(
		"Kinesis Reader Lag",
		TimeSpan.FromMinutes(1)
	);

	public static readonly CustomMetric KinesisStreamThrottles = new(
		"Kinesis Reader Throttles",
		TimeSpan.FromMinutes(1)
	);
}
		
	