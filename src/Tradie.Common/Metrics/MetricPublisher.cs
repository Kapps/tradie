using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Runtime.InteropServices;

namespace Tradie.Common.Metrics;

/// <summary>
/// A service to publish metrics on a periodic basis, caching intermediate values.
/// </summary>
public interface IMetricPublisher {
	/// <summary>
	/// Publishes a new value for the given metric.
	/// If this instance has already updated that value too recently, the value is cached until a future call.
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
	public CloudWatchMetricPublisher(IAmazonCloudWatch cloudWatch, ILogger<CloudWatchMetricPublisher> logger) {
		this._cloudWatch = cloudWatch;
		this._logger = logger;
		this._lastPublishTimes = new();
	}

	private async Task PublishMetricValues(CustomMetric metric, CustomMetricDimension[] dimensions, List<double> values,
		CancellationToken cancellationToken) {
		
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
					Values = values,
					TimestampUtc = DateTime.UtcNow,
				}
			}
		};
		
		var resp = await this._cloudWatch.PutMetricDataAsync(request, cancellationToken);
		this._logger.LogDebug("Published {Count} values for metric {Metric}: {Status}", values.Count, metric.Name, resp.HttpStatusCode);
		
		if(resp.HttpStatusCode != HttpStatusCode.OK) {
			throw new Exception($"CloudWatch failed to publish metric data: {resp.HttpStatusCode}");
		}
		
		this._lastPublishTimes[metric] = DateTime.Now;
		values.Clear();
	}
	
	public Task PublishMetric(CustomMetric metric, CustomMetricDimension[] dimensions, double value, CancellationToken cancellationToken) {
		ref var values =
			ref CollectionsMarshal.GetValueRefOrAddDefault(this._pendingValuesForMetric, metric, out bool exists);
		if(!exists) {
			values = new List<double>();
		}
		
		values!.Add(value);
		
		if(this._lastPublishTimes.TryGetValue(metric, out var lastPublished)) {
			if(values.Count < 150 && DateTime.Now - lastPublished < metric.Interval) {
				this._logger.LogTrace("Skipping publishing of metric {Metric} because it was published too recently", metric.Name);
				return Task.CompletedTask;
			}
		}

		return PublishMetricValues(metric, dimensions, values, cancellationToken);
	}

	private readonly IAmazonCloudWatch _cloudWatch;
	private readonly ILogger<CloudWatchMetricPublisher> _logger;
	private readonly Dictionary<CustomMetric, DateTime> _lastPublishTimes;
	private readonly Dictionary<CustomMetric, List<double>> _pendingValuesForMetric = new();
}