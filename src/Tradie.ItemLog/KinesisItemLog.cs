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
	public KinesisItemLog(
		KinesisStreamReference streamReference,
		IKinesisRecordReader kinesisReader,
		IStashTabSerializer stashTabSerializer,
		IPoisonPillReporter poisonPillReporter
	) {
		this._streamReference = streamReference;
		this._kinesisReader = kinesisReader ?? throw new ArgumentNullException(nameof(kinesisReader));
		this._stashTabSerializer = stashTabSerializer ?? throw new ArgumentNullException(nameof(stashTabSerializer));
		this._poisonPillReporter = poisonPillReporter ?? throw new ArgumentNullException(nameof(this._poisonPillReporter));
	}
	
	public async IAsyncEnumerable<LogRecord> GetItems(ItemLogOffset offset, [EnumeratorCancellation] CancellationToken cancellationToken) {
		await foreach(var record in this._kinesisReader.GetItems(this._streamReference, offset, cancellationToken)) {
			AnalyzedStashTab tab;
			try {
				tab = await this._stashTabSerializer.Deserialize(record.Data, cancellationToken);
			} catch(Exception ex) {
				await this._poisonPillReporter.ReportPoisonedMessage(new ItemLogOffset(record.SequenceNumber), $"Failed to deserialize contents: {ex}", cancellationToken);
				continue;
			}

			//Console.WriteLine($"Deserialized tab {tab.StashTabId}");
			yield return new LogRecord(new ItemLogOffset(record.SequenceNumber), tab);
		}
	}
	
	private readonly KinesisStreamReference _streamReference;
	private readonly IKinesisRecordReader _kinesisReader;
	private readonly IStashTabSerializer _stashTabSerializer;
	private readonly IPoisonPillReporter _poisonPillReporter;
}