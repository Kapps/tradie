using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Microsoft.Extensions.Logging;
using System.Text;
using Tradie.Common;

namespace Tradie.Analyzer.Stores;

/// <summary>
/// Provides storage for analyzed items.
/// </summary>
public interface IAnalyzedStashTabStore {
	/// <summary>
	/// Writes all of the analyzed items for this tab to the store.
	/// </summary>
	public Task WriteItems(AnalyzedStashTab tab);
}

/// <summary>
/// An IAnalyzedItemStore that writes all items to an S3 bucket in compressed JSON format.
/// </summary>
public class KinesisAnalyzedStashTabStore : IAnalyzedStashTabStore {
	/// <summary>
	/// Creates a new KinesisAnalyzedStashTabStore with the given compressor used for compressing JSON documents.
	/// </summary>
	public KinesisAnalyzedStashTabStore(
		ILogger<KinesisAnalyzedStashTabStore> logger,
		IAmazonKinesis kinesisClient,
		IStashTabSerializer serializer
	) {
		this._kinesisClient = kinesisClient;
		this._serializer = serializer;
		this._logger = logger;
	}
	
	public async Task WriteItems(AnalyzedStashTab tab) {
		byte[] serialized = this._serializer.Serialize(tab);
		
		var req = new PutRecordRequest() {
			Data = new MemoryStream(serialized),
			PartitionKey = "default",
			StreamName = TradieConfig.AnalyzedItemStreamName,
		};

		var resp = await this._kinesisClient.PutRecordAsync(req);
		this._logger.LogDebug("Stash tab with ID {id} serialized as sequence number {sequence}.", tab.StashTabId, resp.SequenceNumber);
	}

	private readonly IStashTabSerializer _serializer;
	private readonly IAmazonKinesis _kinesisClient;
	private readonly ILogger<KinesisAnalyzedStashTabStore> _logger;
}