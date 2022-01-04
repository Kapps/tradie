using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Tests.Analyzers;
using Tradie.Common;
using Tradie.Common.Tests;
using static Tradie.TestUtils.TestUtils;

namespace Tradie.Analyzer.Tests.Dispatch; 

[TestClass]
public class AnalyzedStashTabStoreTests {
	[TestMethod]
	public async Task Send_WithItems() {
		var logger = CreateLogger<AnalyzedStashTabKinesisDispatcher>();
		var kinesis = new Mock<IAmazonKinesis>(MockBehavior.Strict);
		var serializer = new Mock<IStashTabSerializer>(MockBehavior.Strict);
		var dispatcher = new AnalyzedStashTabKinesisDispatcher(logger, kinesis.Object, serializer.Object);
		var analyzedTab = new AnalyzedStashTab("foo", Array.Empty<ItemAnalysis>());

		Stream? recordStream = null;

		serializer.Setup(c => c.Serialize(analyzedTab, It.IsAny<Stream>(), default))
			.Callback((AnalyzedStashTab _, Stream stream, CancellationToken _) => recordStream = stream)
			.Returns(Task.CompletedTask);
		
		kinesis.Setup(c => c.PutRecordAsync(It.Is<PutRecordRequest>(req =>
			req.Data == (MemoryStream)recordStream! && req.PartitionKey == "default" &&
			req.StreamName == TradieConfig.AnalyzedItemStreamName
		), CancellationToken.None)).ReturnsAsync(new PutRecordResponse() {
			SequenceNumber = "12"
		});

		await dispatcher.DispatchTab(analyzedTab);
		
		serializer.VerifyAll();
		kinesis.VerifyAll();
	} 
}