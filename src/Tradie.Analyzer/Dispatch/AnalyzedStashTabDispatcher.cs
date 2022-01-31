using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Microsoft.Extensions.Logging;
using System.Text;
using Tradie.Common;

namespace Tradie.Analyzer.Dispatch;

/// <summary>
/// Provides storage for analyzed items.
/// </summary>
public interface IAnalyzedStashTabDispatcher {
	/// <summary>
	/// Append this version of this stash tab to the store.
	/// </summary>
	public Task DispatchTab(AnalyzedStashTab tab, CancellationToken cancellationToken = default);
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
	}
	
	public async Task DispatchTab(AnalyzedStashTab tab, CancellationToken cancellationToken = default) {
		await using var stream = new MemoryStream();
		await this._serializer.Serialize(tab, stream, cancellationToken);
		var req = new PutRecordRequest() {
			Data = stream,
			PartitionKey = "default",
			StreamName = TradieConfig.AnalyzedItemStreamName,
		};

		var resp = await this._kinesisClient.PutRecordAsync(req, cancellationToken);
		this._logger.LogDebug("Stash tab with ID {id} serialized as sequence number {sequence}.", tab.StashTabId, resp.SequenceNumber);
	}

	private readonly IStashTabSerializer _serializer;
	private readonly IAmazonKinesis _kinesisClient;
	private readonly ILogger<AnalyzedStashTabKinesisDispatcher> _logger;
}