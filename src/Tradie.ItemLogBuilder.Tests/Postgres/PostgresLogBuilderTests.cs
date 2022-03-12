using MessagePack.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.ItemLogBuilder;
using Tradie.ItemLogBuilder.Postgres;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests.Postgres;

[TestClass]
public class PostgresLogBuilderTests : TestBase {
	protected override void Initialize() {
		var logger = TestUtils.TestUtils.CreateLogger<IItemLogBuilder>();
		this._context = new();
		this._logBuilder = new PostgresLogBuilder(logger, this._tabRepo.Object, this._context);
	}

	protected override void Cleanup() {
		base.Cleanup();
		this._context.Dispose();
	}

	protected override void DisposeContexts() {
		// no-op; manual transaction management
	}

	protected override void InstantiateContexts() {
		// no-op; manual transaction management
	}

	[TestMethod]
	public async Task TestAppendEntries_Empty() {
		this._tabRepo.Setup(c =>
				c.LogTabs(AsyncEnumerable.Empty<AnalyzedStashTab>().DeepMatcher(), CancellationToken.None))
			.Returns(AsyncEnumerable.Empty<long>());
		var records = Array.Empty<LogRecord>().ToAsyncEnumerable();
		await this._logBuilder.AppendEntries(records, CancellationToken.None);
	}

	[TestMethod]
	[Ignore("Needs fixing and implementing.")]
	public Task TestAppendEntries_MultiRecords_WithDups() {
		return Task.CompletedTask;
		/*var testNow = DateTime.Now;
		var cancellationToken = TestUtils.TestUtils.CreateCancellationToken();
		
		var records = new LogRecord[] {
			CreateRecord("first"),
			CreateRecord("second"),
			CreateRecord("first")
		};
		var first = records[2].StashTab;
		var second = records[1].StashTab;

		var loggedTabs = new LoggedStashTab[] {
			new("first", testNow, testNow, first.AccountName, first.LastCharacterName, first.Name, first.League),
			new("second", testNow, testNow, second.AccountName, second.LastCharacterName, second.Name, second.League),
		};

		this._tabRepo.Setup(c => c.LogTabs(new[] {
			records[2].StashTab,
			records[1].StashTab
		}.ToAsyncEnumerable().DeepMatcher(), cancellationToken));
		
		this._itemRepo.Setup(c=>c.LogItems(new LoggedItem[] {
			new(first.Items[0].ItemId, )
		}))*/
	}

	private PostgresLogBuilder _logBuilder = null!;
	private Mock<ILoggedTabRepository> _tabRepo = null!;
	private AnalysisContext _context = null!;
}