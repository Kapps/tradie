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
		var logger = TestUtils.TestUtils.CreateLogger<PostgresLogBuilder>();
		this._context = new();
		this._logBuilder = new PostgresLogBuilder(logger, this._tabRepo.Object, this._itemRepo.Object, this._context);
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
		var records = Array.Empty<LogRecord>().ToAsyncEnumerable();
		await this._logBuilder.AppendEntries(records, CancellationToken.None);
		// No inserts, transactions, etc.
	}

	[TestMethod]
	public async Task TestAppendEntries_NoItems() {
		var cancellationToken = TestUtils.TestUtils.CreateCancellationToken();
		var records = new LogRecord[] {
			new(new ItemLogOffset("a"), new AnalyzedStashTab("first", "first", null, null, null, null, Array.Empty<ItemAnalysis>())),
		};
		var mappings = new TabMapping[] {new("first", 1)};

		this._tabRepo.Setup(c => c.LogTabs(
			new[] { records[0].StashTab}.ToAsyncEnumerable().DeepMatcher(),
			cancellationToken)
		).Returns(mappings.ToAsyncEnumerable());

		this._itemRepo.Setup(c =>
			c.LogItems(
				mappings.DeepMatcher(), 
				Array.Empty<LoggedItem>().ToAsyncEnumerable().DeepMatcher(), 
				cancellationToken)
			).Returns(Array.Empty<LoggedItem>().ToAsyncEnumerable());
		await this._logBuilder.AppendEntries(records.ToAsyncEnumerable(), cancellationToken);
	}
	
	[TestMethod]
	public async Task TestAppendEntries_WithDupsAndItems() {
		var cancellationToken = TestUtils.TestUtils.CreateCancellationToken();
		var records = new LogRecord[] {
			new(new ItemLogOffset("a"), new AnalyzedStashTab("first", "first", null, null, null, null, Array.Empty<ItemAnalysis>())),
			new(new ItemLogOffset("b"), new AnalyzedStashTab("first", "first", null, null, null, null, Array.Empty<ItemAnalysis>())),
		};
		var mappings = new TabMapping[] {new("first", 1)};
		var items = new LoggedItem[] {
			new("first", 2, new AnalyzedPropertyCollection()),
		};
		this._tabRepo.Setup(c => c.LogTabs(
				new[] { records[1].StashTab}.ToAsyncEnumerable().DeepMatcher(),
				cancellationToken)
			).Returns(mappings.ToAsyncEnumerable());

		this._itemRepo.Setup(c =>
			c.LogItems(
				mappings.DeepMatcher(), 
				items.ToAsyncEnumerable().DeepMatcher(), 
				cancellationToken)
			).Returns(Array.Empty<LoggedItem>().ToAsyncEnumerable());

		await this._logBuilder.AppendEntries(records.ToAsyncEnumerable(), cancellationToken);
	}

	private PostgresLogBuilder _logBuilder = null!;
	private Mock<ILoggedTabRepository> _tabRepo = null!;
	private Mock<ILoggedItemRepository> _itemRepo = null!;
	private AnalysisContext _context = null!;
}