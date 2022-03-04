using Tradie.Common;

namespace Tradie.ItemLog;

public static class ItemLogMetrics {
	public static readonly CustomMetric KinesisStreamLag = new CustomMetric(
		"Kinesis Reader Lag",
		TimeSpan.FromMinutes(1)
	);
}
		
	