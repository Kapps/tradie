using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.ItemLogBuilder.Postgres;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests.Postgres {
	[TestClass]
	public class ItemLogRepositoryTests : TestBase {
		protected override void Initialize() {
			this._repo = new PostgresLoggedItemRepository(this._context);
		}

		protected override void Cleanup() {
			this._repo.DisposeAsync().AsTask().Wait();
		}

		[TestMethod]
		public async Task TestStashUpsert_Empty() {
			var items = AsyncEnumerable.Empty<LoggedItem>();
			var results = this._repo.LogItems(items, CancellationToken.None);
			Assert.IsFalse(await results.AnyAsync());
		}

		[TestMethod]
		public async Task TestItemUpserts_InsertsOnly() {
			var logged = new LoggedItem("foo", 12, Encoding.UTF8.GetBytes("{ \"foo\": \"bar\" }"));
			var insertedItems = await this._repo.LogItems(new[] { logged }.ToAsyncEnumerable(), CancellationToken.None)
				.ToArrayAsync();

			Assert.AreEqual(1, insertedItems.Length);

			var inserted = insertedItems[0];
			var retrieved = await this._context.LoggedItems.SingleAsync(c => c.Id == inserted.Id);
			inserted.ShouldDeepEqual(retrieved);
		}

		private AnalysisContext _context = null!;
		private PostgresLoggedItemRepository _repo = null!;
	}
}