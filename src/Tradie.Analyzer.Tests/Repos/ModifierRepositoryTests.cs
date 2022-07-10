using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Repos; 

[TestClass]
public class ModifierRepositoryTest : TestBase {
	protected override void Initialize() {
		this._repo = new ModifierDbRepository(this._context);
	}

	[TestMethod]
	public async Task TestModifierRepo_EmptyDataSet() {
		var results = await this._repo.RetrieveAll();
		Assert.AreEqual(0, results.Count(c=>c.Kind != ModifierKind.Pseudo));

		results = await this._repo.LoadByModHash(new[] { 123UL, 456UL });
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

		var results = await this._repo.LoadByModHash(new[] { mod.ModHash });
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

		var returned = await this._context.Modifiers.SingleAsync(c=>c.Kind != ModifierKind.Pseudo);

		returned.WithDeepEqual(mod)
			.IgnoreProperty<Modifier>(c=>c.Id)
			.SkipDefault<Modifier>()
			.Assert();
		
		Assert.AreNotEqual(0, mod.Id);
	}

	private ModifierDbRepository _repo = null!;
	private AnalysisContext _context = null!;
}