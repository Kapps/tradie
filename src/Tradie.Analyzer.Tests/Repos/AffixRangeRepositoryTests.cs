using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Repos;

[TestClass]
public class AffixRangeRepositoryTests : TestBase {
	protected override void Initialize() {
		this._repo = new AffixRangeRepository(this._context);

		this._context.AffixRanges.RemoveRange(this._context.AffixRanges.ToArray());
		this._context.SaveChanges();
	}

	[TestMethod]
	public async Task TestEmpty() {
		await _repo.UpsertRanges(Array.Empty<AffixRange>(), CancellationToken.None);
		Assert.AreEqual(await this._context.AffixRanges.CountAsync(), 0);
	}

	[TestMethod]
	public async Task TestInserts() {
		var ranges = new AffixRange[] {
			new(1, null, null, AffixRangeEntityKind.Modifier, ModKindCategory.Enchant),
			new(1, 12, 34, AffixRangeEntityKind.Modifier, ModKindCategory.Explicit)
		};

		await _repo.UpsertRanges(ranges, CancellationToken.None);

		var inserted = await _context.AffixRanges.ToArrayAsync();
		inserted.ShouldDeepEqual(ranges);
	}

	[TestMethod]
	public async Task TestUpdates() {
		var ranges = new AffixRange[] {
			new(1, null, null, AffixRangeEntityKind.Modifier, ModKindCategory.Enchant),
			new(1, 12, 34, AffixRangeEntityKind.Modifier, ModKindCategory.Explicit)
		};

		this._context.AffixRanges.AddRange(ranges);
		await this._context.SaveChangesAsync();

		ranges[1] = ranges[1] with { MaxValue = 46 };
		ranges[0] = ranges[0] with { MinValue = 4 };

		await _repo.UpsertRanges(ranges, CancellationToken.None);

		var inserted = await _context.AffixRanges.ToArrayAsync();
		inserted.ShouldDeepEqual(ranges);
	}

	[TestMethod]
	public async Task TestDeadlock() {
		this.DisposeContexts();

		const int NumTasks = 100;
		var tasks = new List<Task>();
		var rng = new Random();
		for(int i = 0; i < NumTasks; i++) {
			tasks.Add(Task.Run(async () => {
				var ranges = Enumerable.Range(1, 100)
					.Select(c => new AffixRange((ulong)rng.Next(0, 50), rng.Next(0, 10), rng.Next(0, 10), AffixRangeEntityKind.Modifier, ModKindCategory.Implicit))
					.DistinctBy(c => c.ModHash);
				await using var context = new AnalysisContext();
				await using var conn = await context.Database.BeginTransactionAsync();
				await using var repo = new AffixRangeRepository(context);
				await repo.UpsertRanges(ranges, CancellationToken.None);
				await conn.RollbackAsync();
			}));
		}

		await Task.WhenAll(tasks);
	}

	private AnalysisContext _context = null!;
	private AffixRangeRepository _repo = null!;
}