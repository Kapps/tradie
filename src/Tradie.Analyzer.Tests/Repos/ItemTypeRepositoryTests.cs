using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
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
		var results = await this._repo.RetrieveAll(CancellationToken.None);
		Assert.IsTrue(results.Length == 0);
		results = await this._repo.LoadByNames(new[] {"foo", "bar"}, CancellationToken.None);
		Assert.IsTrue(results.Length == 0);
	}
	
	[TestMethod]
	public async Task TestItemTypeRepo_LoadByHash() {
		var itemType = new ItemType() {
			Category = "my category",
			Height = 3,
			Name = "foo",
			Subcategories = new[] { "bar" },
			Width = 4,
			Requirements = new Requirements() {
				Dex = 1, Int = 2, Str = 3, Level = 4
			},
			IconUrl = "about:blank"
		};

		this._context.ItemTypes.Add(itemType);
		await this._context.SaveChangesAsync();

		var results = await this._repo.LoadByNames(new[] { itemType.Name }, CancellationToken.None);
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
			Subcategories = new[] { "bar" },
			Width = 4,
			Requirements = new Requirements() {
				Dex = 1, Int = 2, Str = 3, Level = 4
			},
			IconUrl = "about:blank"
		};

		await this._repo.Insert(new[] { itemType }, CancellationToken.None);

		var returned = await this._context.ItemTypes.SingleAsync();

		returned.WithDeepEqual(itemType)
			.IgnoreProperty<ItemType>(c=>c.Id)
			.SkipDefault<ItemType>()
			.Assert();
		
		Assert.AreNotEqual(0, itemType.Id);
	}
	
	[TestMethod]
	public async Task TestItemTypeRepo_Updates() {
		var itemType = new ItemType() {
			Category = "my category",
			Height = 3,
			Name = "foo",
			Subcategories = new[] { "bar" },
			Width = 4,
			Requirements = new Requirements() {
				Dex = 1, Int = 2, Str = 3, Level = 4
			},
			IconUrl = "about:blank"
		};

		this._context.ItemTypes.Add(itemType);
		await this._context.SaveChangesAsync();

		itemType.IconUrl = "updated";
			
		await this._repo.Update(new[] { itemType }, CancellationToken.None);

		var fromDb = await this._context.ItemTypes.SingleAsync(c => c.Id == itemType.Id);
		
		Assert.AreEqual("updated", fromDb!.IconUrl);
	}

	private ItemTypeDbRepository _repo = null!;
	private AnalysisContext _context = null!;
}