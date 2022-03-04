namespace Tradie.Common;

/// <summary>
/// Represents a numeric metric in that has values pushed to it at a specific timeframe.
/// </summary>
/// <param name="Namespace">The namespace that contains this metric; usually set to DefaultNamespace.</param>
/// <param name="Name">The human-readable name of this metric.</param>
/// <param name="Dimensions">Any custom dimensions, such as an EC2 instance ID or other identifiers; max of 10 values.</param>
/// <param name="Interval">The interval at which this metric should have values published to it.</param>
public record CustomMetric(
	string Name,
	TimeSpan Interval,
	string Namespace = CustomMetric.DefaultNamespace
) {
	/// <summary>
	/// The default namespace value to use for all CloudWatch metrics.
	/// </summary>
	public const string DefaultNamespace = "tradie/";
}

/// <summary>
/// A custom dimension to use when publishing a metric, such as an EC2 instance ID or other unique identifier.
/// </summary>
/// <param name="Name">The name for the custom dimension, such as EC2 Instance ID.</param>
/// <param name="Value">The value of the custom dimension, such as i-123123.</param>
public record struct CustomMetricDimension(
	string Name,
	string Value
);