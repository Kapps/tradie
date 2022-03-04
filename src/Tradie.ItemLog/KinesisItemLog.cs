using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using System.Runtime.CompilerServices;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;

namespace Tradie.ItemLog; 

/// <summary>
/// An IItemLog implementation that reads items from a Kinesis stream from a given offset.
/// </summary>
public class KinesisItemLog : IItemLog {
	/// <summary>
	/// Creates a new KinesisItemLog that starts reading records from the given stream reference.
	/// </summary>
	public KinesisItemLog(KinesisStreamReference streamReference, IKinesisRecordReader kinesisReader, IStashTabSerializer stashTabSerializer) {
		this._streamReference = streamReference;
		this._kinesisReader = kinesisReader ?? throw new ArgumentNullException(nameof(kinesisReader));
		this._stashTabSerializer = stashTabSerializer ?? throw new ArgumentNullException(nameof(stashTabSerializer));
	}
	
	public async IAsyncEnumerable<LogRecord> GetItems(ItemLogOffset offset, [EnumeratorCancellation] CancellationToken cancellationToken) {
		await foreach(var record in this._kinesisReader.GetItems(this._streamReference, offset, cancellationToken)) {
			var tab = await this._stashTabSerializer.Deserialize(record.Data, cancellationToken);
			//Console.WriteLine($"Deserialized tab {tab.StashTabId}");
			yield return new LogRecord(new ItemLogOffset(record.SequenceNumber), tab);
		}
	}
	
	private readonly KinesisStreamReference _streamReference;
	private readonly IKinesisRecordReader _kinesisReader;
	private readonly IStashTabSerializer _stashTabSerializer;
}