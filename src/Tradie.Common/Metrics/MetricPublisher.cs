using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using System.Net;

namespace Tradie.Common;

/// <summary>
/// A service to publish metrics on a periodic basis, discarding intermediate results.
/// </summary>
public interface IMetricPublisher {
	/// <summary>
	/// Publishes a new value for the given metric.
	/// If this instance has already updated that value too recently, the new update is discarded.
	/// </summary>
	Task PublishMetric(CustomMetric metric, CustomMetricDimension[] dimensions, double value, CancellationToken cancellationToken);
}

/// <summary>
/// A metric publisher that publishes data to CloudWatch.
/// </summary>
public class CloudWatchMetricPublisher : IMetricPublisher {
	/// <summary>
	/// Creates a new CloudWatchMetricPublisher that uses the given CloudWatch interface to publish data.
	/// </summary>
	public CloudWatchMetricPublisher(IAmazonCloudWatch cloudWatch) {
		this._cloudWatch = cloudWatch;
		this._lastPublishTimes = new();
	}
	
	public async Task PublishMetric(CustomMetric metric, CustomMetricDimension[] dimensions, double value, CancellationToken cancellationToken) {
		if(this._lastPublishTimes.TryGetValue(metric, out var lastPublished)) {
			if(DateTime.Now - lastPublished < metric.Interval) {
				return;
			}
		}
		var request = new PutMetricDataRequest() {
			Namespace = metric.Namespace,
			MetricData = new() {
				new MetricDatum() {
					Dimensions = dimensions.Select(c=>new Dimension() {
						Name = c.Name,
						Value = c.Value
					}).ToList(),
					Unit = StandardUnit.None,
					MetricName = metric.Name,
					Value = value,
					TimestampUtc = DateTime.UtcNow,
				}
			}
		};
		
		await this._cloudWatch.PutMetricDataAsync(request, cancellationToken);
	}

	private readonly IAmazonCloudWatch _cloudWatch;
	private readonly Dictionary<CustomMetric, DateTime> _lastPublishTimes;
}