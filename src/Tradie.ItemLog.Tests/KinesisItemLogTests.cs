using Amazon.Kinesis.Model;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Analyzer.Dispatch;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests;

[TestClass]
public class KinesisItemLogTests : TestBase {
	[TestMethod]
	public async Task TestGetItems_SingleItem() {
		var ms = new MemoryStream();
		this._kinesisRecordReader.Setup(c => c.GetItems(this._streamReference, new ItemLogOffset("foo"), CancellationToken.None))
			.Returns(new Record[] {
				new() {
					SequenceNumber = "123",
					Data = ms,
				}
			}.ToAsyncEnumerable());

		var tab = new AnalyzedStashTab("id", "name", null, "acc", "league", "kind", new[] {
			new ItemAnalysis("itemId"),
		});
		this._stashTabSerializer.Setup(c => c.Deserialize(ms, CancellationToken.None))
			.ReturnsAsync(tab);

		var res = await this._itemLog.GetItems(new ItemLogOffset("foo"), CancellationToken.None)
			.ToArrayAsync();

		res.ShouldDeepEqual(new[] {new LogRecord(new ItemLogOffset("123"), tab)});
	}
	
	[TestMethod]
	public async Task TestGetItems_PoisonPill() {
		var ms = new MemoryStream();
		this._kinesisRecordReader.Setup(c => c.GetItems(this._streamReference, new ItemLogOffset("foo"), CancellationToken.None))
			.Returns(new Record[] {
				new() {
					SequenceNumber = "123",
					Data = ms,
				}
			}.ToAsyncEnumerable());
		
		this._stashTabSerializer.Setup(c => c.Deserialize(ms, CancellationToken.None))
			.ThrowsAsync(new DataException());
		this._poisonPillReporter.Setup(c =>
				c.ReportPoisonedMessage(new ItemLogOffset("123"), It.IsAny<string>(), CancellationToken.None))
			.Returns(Task.CompletedTask);
		
		var res = await this._itemLog.GetItems(new ItemLogOffset("foo"), CancellationToken.None)
			.ToArrayAsync();

		res.ShouldDeepEqual(Array.Empty<LogRecord>());
	}

	protected override void Initialize() {
		this._streamReference = new KinesisStreamReference("shard", "stream");
		this._itemLog = new KinesisItemLog(this._streamReference, this._kinesisRecordReader.Object, this._stashTabSerializer.Object, this._poisonPillReporter.Object);
	}

	private Mock<IStashTabSerializer> _stashTabSerializer = null!;
	private Mock<IKinesisRecordReader> _kinesisRecordReader = null!;
	private Mock<IPoisonPillReporter> _poisonPillReporter = null!;
	private KinesisStreamReference _streamReference;
	private KinesisItemLog _itemLog = null!;
}