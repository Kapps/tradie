using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Tests.Analyzers;
using Tradie.Common;
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
		var analyzedTab = new AnalyzedStashTab("foo", "name", null, "acc", "Scourge", "Standard", Array.Empty<ItemAnalysis>());

		Stream? recordStream = null;

		serializer.Setup(c => c.Serialize(analyzedTab, It.IsAny<Stream>(), default))
			.Callback((AnalyzedStashTab _, Stream stream, CancellationToken _) => recordStream = stream)
			.Returns(ValueTask.CompletedTask);

		await dispatcher.DispatchTab(analyzedTab);
		
		kinesis.Setup(c => c.PutRecordsAsync(new PutRecordsRequest() {
			StreamName = TradieConfig.AnalyzedItemStreamName,
			Records = new() {
				new() {
					Data = (MemoryStream)recordStream!,
					PartitionKey = "default"
				}
			}
		}.DeepMatcher(), CancellationToken.None)).ReturnsAsync(new PutRecordsResponse() {
			Records = new List<PutRecordsResultEntry>() {
				new() {
					SequenceNumber = "12"
				}
			}
		});
		
		await dispatcher.Flush();
		
		
		serializer.VerifyAll();
		kinesis.VerifyAll();
	} 
}