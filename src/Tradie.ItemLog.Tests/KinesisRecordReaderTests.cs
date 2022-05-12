using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Common;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests; 

[TestClass]
public class KinesisRecordReaderTests : TestBase {
	[TestMethod]
	public async Task TestRead_FromTrimHorizon() {
		this._kinesisClient.Setup(c => c.GetShardIteratorAsync(new GetShardIteratorRequest() {
			ShardId = TradieConfig.LogBuilderShardId,
			StreamName = TradieConfig.AnalyzedItemStreamName,
			ShardIteratorType = ShardIteratorType.TRIM_HORIZON,
		}.DeepMatcher(), CancellationToken.None)).ReturnsAsync(new GetShardIteratorResponse() {
			ShardIterator = "foo",
		});

		var records = new List<Record>() {
			new() {
				Data = new MemoryStream(Encoding.UTF8.GetBytes("data"))
			}
		};

		bool firstCall = true;
		this._kinesisClient.Setup(c => c.GetRecordsAsync(new GetRecordsRequest() {
			Limit = TradieConfig.ItemStreamBatchSize,
			ShardIterator = "foo",
		}.DeepMatcher(), CancellationToken.None)).ReturnsAsync(() => {
			if(!firstCall) {
				return new GetRecordsResponse() {
					Records = new List<Record>(),
					MillisBehindLatest = 0
				};
			}
			firstCall = false;
			return new GetRecordsResponse() {
				Records = records,
				NextShardIterator = "foo",
				MillisBehindLatest = 4200,
			};
		});

		this._metricPublisher.Setup(c => c.PublishMetric(It.IsAny<CustomMetric>(), new CustomMetricDimension[] {
				new("Stream Name", TradieConfig.AnalyzedItemStreamName),
				new("Shard Id", TradieConfig.LogBuilderShardId)
			}.DeepMatcher(), It.IsIn(0, 4.2), CancellationToken.None))
			.Returns(Task.CompletedTask);

		var streamRef = new KinesisStreamReference(TradieConfig.LogBuilderShardId, TradieConfig.AnalyzedItemStreamName);

		var res = await this._recordReader.GetItems(streamRef, new ItemLogOffset(), CancellationToken.None)
			.ToArrayAsync();
		
		res.ShouldDeepEqual(records.ToArray());
	}
	
	[TestMethod]
	public async Task TestRead_FromSequenceStart() {
		this._kinesisClient.Setup(c => c.GetShardIteratorAsync(new GetShardIteratorRequest() {
			ShardId = TradieConfig.LogBuilderShardId,
			StreamName = TradieConfig.AnalyzedItemStreamName,
			ShardIteratorType = ShardIteratorType.AFTER_SEQUENCE_NUMBER,
			StartingSequenceNumber = "123",
		}.DeepMatcher(), CancellationToken.None)).ReturnsAsync(new GetShardIteratorResponse() {
			ShardIterator = "foo",
		});

		var records = new List<Record>() {
			new() {
				Data = new MemoryStream(Encoding.UTF8.GetBytes("data"))
			}
		};

		bool firstCall = true;
		this._kinesisClient.Setup(c => c.GetRecordsAsync(new GetRecordsRequest() {
			Limit = TradieConfig.ItemStreamBatchSize,
			ShardIterator = "foo",
		}.DeepMatcher(), CancellationToken.None)).ReturnsAsync(() => {
			if(!firstCall) {
				return new GetRecordsResponse() {
					Records = new List<Record>(),
					MillisBehindLatest = 0
				};
			}
			firstCall = false;
			return new GetRecordsResponse() {
				Records = records,
				NextShardIterator = "foo",
				MillisBehindLatest = 840000
			};
		});

		this._metricPublisher.Setup(c => c.PublishMetric(It.IsAny<CustomMetric>(), new CustomMetricDimension[] {
				new("Stream Name", TradieConfig.AnalyzedItemStreamName),
				new("Shard Id", TradieConfig.LogBuilderShardId)
			}.DeepMatcher(), It.IsIn(0, 840.0), CancellationToken.None))
			.Returns(Task.CompletedTask);

		var streamRef = new KinesisStreamReference(TradieConfig.LogBuilderShardId, TradieConfig.AnalyzedItemStreamName);

		var res = await this._recordReader.GetItems(streamRef, new ItemLogOffset("123"), CancellationToken.None)
			.ToArrayAsync();
		
		res.ShouldDeepEqual(records.ToArray());
	}

	protected override void Initialize() {
		this._recordReader = new KinesisRecordReader(this._kinesisClient.Object, this._metricPublisher.Object, TestUtils.TestUtils.CreateLogger<KinesisRecordReader>());
	}
	
	private Mock<IAmazonKinesis> _kinesisClient = null!;
	private Mock<IMetricPublisher> _metricPublisher = null!;
	private KinesisRecordReader _recordReader = null!;
}