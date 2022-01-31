using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Repos; 

[TestClass]
public class ItemTypeRepositoryTest : TestBase {

	protected override void Initialize() {
		this._repo = new ItemTypeDbRepository(this._context);
	}

	[TestMethod]
	public async Task TestItemTypeRepo_EmptyDataSet() {
		var results = await this._repo.RetrieveAll();
		Assert.IsTrue(results.Length == 0);
		results = await this._repo.LoadByNames("foo", "bar");
		Assert.IsTrue(results.Length == 0);
	}
	
	[TestMethod]
	public async Task TestItemTypeRepo_LoadByHash() {
		var itemType = new ItemType() {
			Category = "my category",
			Height = 3,
			Name = "foo",
			Subcategory = "bar",
			Width = 4,
			Requirements = new Requirements() {
				Dex = 1, Int = 2, Str = 3, Level = 4
			},
		};

		this._context.ItemTypes.Add(itemType);
		await this._context.SaveChangesAsync();

		var results = await this._repo.LoadByNames(itemType.Name);
		Assert.IsTrue(results.Length == 1);
		
		results[0].WithDeepEqual(itemType)
			.IgnoreProperty<ItemType>(c=>c.Id)
			.SkipDefault<ItemType>()
			.Assert();
	}

	[TestMethod]
	public async Task TestItemTypeRepo_Insert() {
		var itemType = new ItemType() {
			Category = "my category",
			Height = 3,
			Name = "foo",
			Subcategory = "bar",
			Width = 4,
			Requirements = new Requirements() {
				Dex = 1, Int = 2, Str = 3, Level = 4
			},
		};

		await this._repo.Insert(new[] { itemType });

		var returned = await this._context.ItemTypes.SingleAsync();

		returned.WithDeepEqual(itemType)
			.IgnoreProperty<ItemType>(c=>c.Id)
			.SkipDefault<ItemType>()
			.Assert();
		
		Assert.AreNotEqual(0, itemType.Id);
	}

	private ItemTypeDbRepository _repo = null!;
	private AnalysisContext _context = null!;
}