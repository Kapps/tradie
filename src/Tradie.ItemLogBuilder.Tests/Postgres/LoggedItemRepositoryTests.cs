using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
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
			var tabMappings = Array.Empty<TabMapping>();
			var results = this._repo.LogItems(tabMappings, items, CancellationToken.None);
			Assert.IsFalse(await results.AnyAsync());
		}

		[TestMethod]
		public async Task TestItemUpserts_InsertsOnly() {
			var items = new LoggedItem[] {
				new("foo", 12, new() { { KnownAnalyzers.ItemType, new ItemTypeAnalysis(12) } })
			};
			var tabMappings = new TabMapping[] {
				new("arr", 12)
			};
			var insertedItems = await this._repo.LogItems(
				tabMappings,
				items.ToAsyncEnumerable(),
				CancellationToken.None
			).ToArrayAsync();

			Assert.AreEqual(1, insertedItems.Length);

			var inserted = insertedItems[0];
			var retrieved = await this._context.LoggedItems.SingleAsync(c => c.IdHash == inserted.IdHash);

			inserted.WithDeepEqual(retrieved)
				.SkipDefault<LoggedItem>()
				.SkipDefault<AnalyzedPropertyCollection>()
				.Assert(); 
			
			inserted.WithDeepEqual(items[0])
				.SkipDefault<LoggedItem>()
				.SkipDefault<AnalyzedPropertyCollection>()
				.Assert(); 
		}
		
		[TestMethod]
		public async Task TestItemUpserts_Deletes() {
			var items = new LoggedItem[] {
				new("foo", 12, new() { { KnownAnalyzers.ItemType, new ItemTypeAnalysis(12) } })
			};
			var tabMappings = new TabMapping[] {
				new("arr", 12)
			};

			this._context.LoggedItems.Add(items[0]);
			await this._context.SaveChangesAsync();
			
			var insertedItems = await this._repo.LogItems(
				tabMappings,
				AsyncEnumerable.Empty<LoggedItem>(),
				CancellationToken.None
			).ToArrayAsync();

			Assert.AreEqual(0, insertedItems.Length);
			
			var retrieved = await this._context.LoggedItems.SingleOrDefaultAsync(c => c.IdHash == items[0].IdHash);
			Assert.IsNull(retrieved);
		}
		
		[TestMethod]
		public async Task TestItemUpserts_Updates() {
			var existing = new LoggedItem[] {
				new("foo", 12, new() { { KnownAnalyzers.ItemType, new ItemTypeAnalysis(12) } })
			};
			var updated = new LoggedItem[] {
				new("foo", 24, new() {{KnownAnalyzers.ItemType, new ItemTypeAnalysis(24)}})
			};
			var tabMappings = new TabMapping[] {
				new("arr", 12),
				new("barr", 24)
			};

			this._context.LoggedItems.AddRange(existing);
			await this._context.SaveChangesAsync();
			
			var insertedItems = await this._repo.LogItems(
				tabMappings,
				updated.ToAsyncEnumerable(),
				CancellationToken.None
			).ToArrayAsync();
			
			Assert.AreEqual(1, insertedItems.Length);

			var inserted = insertedItems[0];
			var retrieved = await this._context.LoggedItems.SingleOrDefaultAsync(c => c.IdHash == inserted.IdHash);
			
			inserted.WithDeepEqual(retrieved)
				.SkipDefault<LoggedItem>()
				.SkipDefault<AnalyzedPropertyCollection>()
				.Assert(); 
			
			inserted.WithDeepEqual(updated[0])
				.SkipDefault<LoggedItem>()
				.SkipDefault<AnalyzedPropertyCollection>()
				.Assert();
			
			Assert.AreEqual(0, await this._context.LoggedItems.CountAsync(c=>c.StashTabId == 12));
		}

		private AnalysisContext _context = null!;
		private PostgresLoggedItemRepository _repo = null!;
	}
}
