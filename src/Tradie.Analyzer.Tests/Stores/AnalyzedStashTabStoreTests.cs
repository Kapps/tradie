using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Stores;
using Tradie.Analyzer.Tests.Analyzers;
using Tradie.Common;

namespace Tradie.Analyzer.Tests.Stores; 

[TestClass]
public class AnalyzedStashTabStoreTests {
	[TestMethod]
	public async Task Send_WithItems() {
		var logger = TestUtils.CreateLogger<KinesisAnalyzedStashTabStore>();
		var kinesis = new Mock<IAmazonKinesis>(MockBehavior.Strict);
		var serializer = new Mock<IStashTabSerializer>(MockBehavior.Strict);
		var store = new KinesisAnalyzedStashTabStore(logger, kinesis.Object, serializer.Object);

		var analyzedTab = new AnalyzedStashTab("foo", Array.Empty<AnalyzedItem>());

		serializer.Setup(c => c.Serialize(analyzedTab)).Returns(new byte[] {12, 34});
		
		kinesis.Setup(c => c.PutRecordAsync(It.Is<PutRecordRequest>(req =>
			req.Data.ToArray().IsDeepEqual(new byte[] {12, 34}) && req.PartitionKey == "default" &&
			req.StreamName == TradieConfig.AnalyzedItemStreamName
		), CancellationToken.None)).ReturnsAsync(new PutRecordResponse() {
			SequenceNumber = "12"
		});

		await store.WriteItems(analyzedTab);
		
		serializer.VerifyAll();
		kinesis.VerifyAll();
	} 
}