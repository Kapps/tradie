using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using System.Runtime.CompilerServices;
using Tradie.Common;

namespace Tradie.ItemLog;

/// <summary>
/// A reference to a specific shard within a Kinesis stream.
/// </summary>
/// <param name="ShardId">The ID of the shard to read from within the stream.</param>
/// <param name="StreamName">The Name attribute of the Kinesis stream.</param>
public readonly record struct KinesisStreamReference(string ShardId, string StreamName);

/// <summary>
/// An interface to read Kinesis records from a point within a stream.
/// </summary>
public interface IKinesisRecordReader {
	/// <summary>
	/// Returns all items from the given stream until the end is reached or the cancellation token expires.
	/// </summary>
	IAsyncEnumerable<Record> GetItems(KinesisStreamReference streamReference, ItemLogOffset offset, CancellationToken cancellationToken = default);
}

/// <summary>
/// Provides a wrapper around an AWS Kinesis client to allow for streaming records with throughput and retry management.
/// </summary>
public class KinesisRecordReader : IKinesisRecordReader {
	public KinesisRecordReader(IAmazonKinesis kinesisClient, IMetricPublisher metricPublisher) {
		this._kinesisClient = kinesisClient;
		this._metricPublisher = metricPublisher;
	}
	
	public async IAsyncEnumerable<Record> GetItems(KinesisStreamReference streamReference, ItemLogOffset offset, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
		var iterator = await GetStartingIterator(streamReference, offset, cancellationToken);
		int iterationCount = 0;
		while(!cancellationToken.IsCancellationRequested) {
			GetRecordsResponse? records;
			try {
				records = await GetRecordsFromIterator(streamReference, iterator, cancellationToken);
			} catch(OperationCanceledException) {
				yield break;
			}

			if(records.NextShardIterator == null) {
				yield break;
			}


			await this._metricPublisher.PublishMetric(ItemLogMetrics.KinesisStreamLag, new CustomMetricDimension[] {
				new("Stream Name", streamReference.StreamName),
				new("Shard Id", streamReference.ShardId)
			}, records.MillisBehindLatest / 1000.0, cancellationToken);
			
			iterationCount++;
			if(iterationCount % 50 == 0) {
				Console.WriteLine(
					$"Next shard iterator is {records.NextShardIterator} (roughly {records.MillisBehindLatest}ms behind latest).");
			}

			foreach(var record in records.Records) {
				yield return record;
			}

			iterator = records.NextShardIterator;
		}
	}
	
	private Task<GetRecordsResponse> GetRecordsFromIterator(KinesisStreamReference streamReference, string iterator, CancellationToken cancellationToken) {
		Task<GetRecordsResponse> GetRecords (int limit) {
			try {
				return this._kinesisClient.GetRecordsAsync(new GetRecordsRequest() {
					Limit = limit,
					ShardIterator = iterator,
				}, cancellationToken);
			} catch(ProvisionedThroughputExceededException) {
				if(limit <= 1) {
					throw new InvalidDataException(
						$"Kinesis stream for {streamReference.StreamName}:{streamReference.ShardId} could not be read at iterator {iterator}.");
				}
				return GetRecords(limit / 2);
			}	
		};

		return GetRecords(TradieConfig.ItemStreamBatchSize);
	}

	private async Task<string> GetStartingIterator(KinesisStreamReference streamReference, ItemLogOffset offset, CancellationToken cancellationToken) {
		var resp = await this._kinesisClient.GetShardIteratorAsync(new GetShardIteratorRequest() {
			ShardId = streamReference.ShardId,
			StreamName = streamReference.StreamName,
			ShardIteratorType = String.IsNullOrWhiteSpace(offset.Offset) ? ShardIteratorType.TRIM_HORIZON : ShardIteratorType.AFTER_SEQUENCE_NUMBER,
			StartingSequenceNumber = String.IsNullOrWhiteSpace(offset.Offset) ? null : offset.Offset
		}, cancellationToken);
		
		return resp.ShardIterator;
	}
	
	private readonly IAmazonKinesis _kinesisClient;
	private readonly IMetricPublisher _metricPublisher;
}