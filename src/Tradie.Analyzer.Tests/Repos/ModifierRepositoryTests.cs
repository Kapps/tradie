using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.Analyzer.Tests.Repos; 

[TestClass]
public class ModifierRepositoryTest {
	[TestInitialize]
	public async Task Initialize() {
		this._context = new AnalysisContext();
		await this._context.Database.BeginTransactionAsync();
		this._repo = new ModifierDbRepository(this._context);
	}

	[TestCleanup]
	public async Task Cleanup() {
		await this._context.Database.RollbackTransactionAsync();
	}

	[TestMethod]
	public async Task TestModifierRepo_EmptyDataSet() {
		var results = await this._repo.RetrieveAll();
		Assert.IsTrue(results.Length == 0);

		results = await this._repo.LoadByModHash(123UL, 456UL);
		Assert.IsTrue(results.Length == 0);
	}
	
	[TestMethod]
	public async Task TestModifierRepo_LoadByHash() {
		var mod = new Modifier() {
			ModHash = 123,
			ModifierText = "foo",
		};

		this._context.Modifiers.Add(mod);
		await this._context.SaveChangesAsync();

		var results = await this._repo.LoadByModHash(mod.ModHash);
		Assert.IsTrue(results.Length == 1);
		
		results[0].WithDeepEqual(mod)
			.IgnoreProperty<Modifier>(c=>c.Id)
			.SkipDefault<Modifier>()
			.Assert();
	}

	[TestMethod]
	public async Task TestModifierRepo_Insert() {
		var mod = new Modifier() {
			ModHash = 123,
			ModifierText = "foo",
		};

		await this._repo.Insert(new[] { mod });

		var returned = await this._context.Modifiers.SingleAsync();

		returned.WithDeepEqual(mod)
			.IgnoreProperty<Modifier>(c=>c.Id)
			.SkipDefault<Modifier>()
			.Assert();
		
		Assert.AreNotEqual(0, mod.Id);
	}

	private ModifierDbRepository _repo = null!;
	private AnalysisContext _context = null!;
}