using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Common;
using Tradie.ItemLogBuilder;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests {
	[TestClass]
	public class LogStreamerTests : TestBase {
		protected override void Initialize() {
			this._logStreamer = new LogStreamer(TestUtils.TestUtils.CreateLogger<ILogStreamer>(), this._parameterStore.Object);
		}

		[TestMethod]
		public async Task TestBasics() {
			string key = "LogBuilder.Postgres.Offset";
			var offset = new ItemLogOffset("foo");
			var records = new LogRecord[] {
				new(new ItemLogOffset("bar"),
					new AnalyzedStashTab("id", "name", "last", "acc", "League", "kind", Array.Empty<ItemAnalysis>()))
			};
			this._logBuilder.Setup(c => c.Name).Returns("Postgres");
			this._parameterStore.Setup(c => c.GetParameter<string>(key, null))
				.ReturnsAsync(new DynamicParameter<string?>(key, "foo"));
			this._parameterStore.Setup(c => c.SetParameter<string>(key, "bar"))
				.Returns(Task.CompletedTask);
			this._itemLog.Setup(c => c.GetItems(offset, CancellationToken.None))
				.Returns(records.ToAsyncEnumerable());
			this._logBuilder.Setup(c =>
					c.AppendEntries(
						It.Is<IAsyncEnumerable<LogRecord>>(c=>c.ToArrayAsync(CancellationToken.None).Result.SequenceEqual(records)),
						CancellationToken.None
					))
				.Returns(Task.CompletedTask);
			
			await this._logStreamer.CopyItemsFromLog(this._itemLog.Object, this._logBuilder.Object, CancellationToken.None);
		}

		private LogStreamer _logStreamer = null!;
		private Mock<IParameterStore> _parameterStore = null!;
		private Mock<IItemLog> _itemLog = null!;
		private Mock<IItemLogBuilder> _logBuilder = null!;
	}
}