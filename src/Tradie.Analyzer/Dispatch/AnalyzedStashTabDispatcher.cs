using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using Tradie.Common;

namespace Tradie.Analyzer.Dispatch;

/// <summary>
/// Provides storage for analyzed items.
/// </summary>
public interface IAnalyzedStashTabDispatcher {
	/// <summary>
	/// Append this version of this stash tab to the store.
	/// Implementations of this interface are allowed to buffer tabs to be sent off here.
	/// Callers should ensure that Flush is called to ensure tabs are actually sent off.
	/// </summary>
	ValueTask DispatchTab(AnalyzedStashTab tab, CancellationToken cancellationToken = default);

	/// <summary>
	/// Flushes any remaining tabs that have been queued via a call to DispatchTab but not yet sent off.
	/// </summary>
	ValueTask Flush(CancellationToken cancellationToken = default);
}

/// <summary>
/// An IAnalyzedStashTabStore that writes all tabs as records to a Kinesis stream.
/// </summary>
public class AnalyzedStashTabKinesisDispatcher : IAnalyzedStashTabDispatcher {
	/// <summary>
	/// Creates a new KinesisAnalyzedStashTabStore with the given compressor used for compressing JSON documents.
	/// </summary>
	public AnalyzedStashTabKinesisDispatcher(
		ILogger<AnalyzedStashTabKinesisDispatcher> logger,
		IAmazonKinesis kinesisClient,
		IStashTabSerializer serializer
	) {
		this._kinesisClient = kinesisClient;
		this._serializer = serializer;
		this._logger = logger;
		this._queuedRecords = new();
	}

	public async ValueTask DispatchTab(AnalyzedStashTab tab, CancellationToken cancellationToken = default) {
		var entry = new PutRecordsRequestEntry() {
			PartitionKey = "default",
			Data = new MemoryStream()
		};
		
		await this._serializer.Serialize(tab, entry.Data, cancellationToken);

		this._queuedBytes += entry.PartitionKey.Length + (int)entry.Data.Length;
		this._queuedRecords.Add(entry);

		if(this._queuedBytes >= MaxQueuedBytes || this._queuedRecords.Count >= MaxQueuedEntries) {
			await this.Flush(cancellationToken);
		}
	}
	
	public async ValueTask Flush(CancellationToken cancellationToken = default) {
		if(this._queuedRecords.Count == 0) {
			return;
		}
		
		var req = new PutRecordsRequest() {
			StreamName = TradieConfig.AnalyzedItemStreamName,
			Records = this._queuedRecords
		};
		
		var resp = await this._kinesisClient.PutRecordsAsync(req, cancellationToken);

		if(resp.FailedRecordCount > 0) {
			string failures = String.Join(", ", resp.Records.Where(c => c.ErrorMessage != null).Select(c=>$"{c.ErrorMessage} ({c.ErrorCode})"));
			throw new DataException($"Failed to dispatch {resp.FailedRecordCount} records -- failure reasons: {failures}");
		}
		
		this._logger.LogInformation("Dispatched {numTabs} stash tabs totalling {bytes:F3}KB with last sequence number of {sequence}.",
			this._queuedRecords.Count, this._queuedBytes / 1024.0, resp.Records.Last().SequenceNumber);
		
		this._queuedBytes = 0;
		this._queuedRecords.Clear();
	}

	private readonly IStashTabSerializer _serializer;
	private readonly IAmazonKinesis _kinesisClient;
	private readonly ILogger<AnalyzedStashTabKinesisDispatcher> _logger;
	private readonly List<PutRecordsRequestEntry> _queuedRecords;
	private int _queuedBytes;

	private const int MaxQueuedEntries = 450; // Kinesis has a 500 record limit for PutRecords.
	private const int MaxQueuedBytes = 4_000_000; // Kinesis has a 5MB limit for PutRecords.
}