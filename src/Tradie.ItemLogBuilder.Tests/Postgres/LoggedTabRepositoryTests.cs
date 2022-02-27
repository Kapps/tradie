using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.ItemLogBuilder.Postgres;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests.Postgres {
	[TestClass]
	public class LoggedTabRepositoryTests : TestBase {
		protected override void Initialize() {
			this._repo = new PostgresLoggedTabRepository(this._context, TestUtils.TestUtils.CreateLogger<ILoggedTabRepository>());
		}

		protected override void Cleanup() {
			this._repo.DisposeAsync().AsTask().Wait();
		}

		[TestMethod]
		public async Task TestStashUpsert_Empty() {
			var tabs = AsyncEnumerable.Empty<AnalyzedStashTab>();
			var results = this._repo.LogTabs(tabs, CancellationToken.None);
			Assert.IsFalse(await results.AnyAsync());
		}

		[TestMethod]
		public async Task TestStashUpsert_InsertsOnly() {
			var items = new ItemAnalysis[] {
				new("itemId", new Dictionary<ushort, IAnalyzedProperties>() {
					{12, new LogRecordUtils.AnalyzedProps(23)}
				})
			};

			var loggedItems = items.Select(c =>
					new LoggedItem(c.ItemId, new Dictionary<ushort, IAnalyzedProperties>(c.Properties)))
				.ToArray();
			var tabs = new[] {
				new AnalyzedStashTab("foo", "name", null, "acc", "league", "kind", items),
			};
			var insertedTabIds = await this._repo.LogTabs(tabs.ToAsyncEnumerable(), CancellationToken.None)
				.ToArrayAsync();

			Assert.AreEqual(1, insertedTabIds.Length);
			
			var insertedTabs = await this._context.LoggedStashTabs.Where(c => insertedTabIds.Contains(c.Id))
				.ToArrayAsync(CancellationToken.None);

			var inserted = insertedTabs[0];
			inserted.WithDeepEqual(new LoggedStashTab("foo", DateTime.Now, DateTime.Now, "acc", null, "name", "league", loggedItems))
				.IgnoreSourceProperty(c=>c.Id)
				.IgnoreSourceProperty(c=>c.Created)
				.IgnoreSourceProperty(c=>c.LastModified)
				.Assert();
			
			
		}

		private AnalysisContext _context = null!;
		private PostgresLoggedTabRepository _repo = null!;
	}
}