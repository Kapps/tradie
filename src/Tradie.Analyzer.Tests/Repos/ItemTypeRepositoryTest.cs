﻿using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;

namespace Tradie.Analyzer.Tests.Repos; 

[TestClass]
public class ItemTypeRepositoryTest {
	[TestInitialize]
	public async Task Initialize() {
		this._context = new AnalysisContext();
		await this._context.Database.BeginTransactionAsync();
		this._repo = new ItemTypeRepository(this._context);
	}

	[TestCleanup]
	public async Task Cleanup() {
		await this._context.Database.RollbackTransactionAsync();
	}

	[TestMethod]
	public async Task TestItemTypeRepo_EmptyDataSet() {
		var results = await this._repo.RetrieveAll();
		Assert.IsTrue(results.Length == 0);

		results = await this._repo.LoadByNames("foo", "bar");
		Assert.IsTrue(results.Length == 0);
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

	private ItemTypeRepository _repo = null!;
	private AnalysisContext _context = null!;
}