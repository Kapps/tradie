using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.ItemLogBuilder;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests {
	[TestClass]
	public class LogStreamerTests : TestBase {
		protected override void Initialize() {
			this._logStreamer = new LogStreamer();
		}

		[TestMethod]
		public async Task TestBasics() {
			var offset = new ItemLogOffset("foo");
			var records = new LogRecord[] {
				new(offset,
					new AnalyzedStashTab("id", "name", "last", "acc", "League", "kind", Array.Empty<ItemAnalysis>()))
			};
			this._itemLog.Setup(c => c.GetItems(offset, CancellationToken.None))
				.Returns(records.ToAsyncEnumerable());
			this._logBuilder.Setup(c =>
					c.AppendEntries(records.ToAsyncEnumerable().DeepMatcher(), CancellationToken.None))
				.Returns(Task.CompletedTask);
			
			await this._logStreamer.CopyItemsFromLog(this._itemLog.Object, this._logBuilder.Object, offset, CancellationToken.None);
		}

		private LogStreamer _logStreamer = null!;
		private Mock<IItemLog> _itemLog = null!;
		private Mock<IItemLogBuilder> _logBuilder = null!;
	}
}