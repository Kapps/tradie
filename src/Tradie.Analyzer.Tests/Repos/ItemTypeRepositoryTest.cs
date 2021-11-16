using DeepEqual.Syntax;
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
	public void Initialize() {
		this.context = new AnalysisContext();
		this.repo = new ItemTypeRepository(this.context);
	}

	[TestMethod]
	public async Task TestItemTypeRepo_EmptyDataSet() {
		var results = await repo.RetrieveAll();
		Assert.IsTrue(results.Length == 0);

		results = await repo.LoadByNames("foo", "bar");
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

		await this.repo.Upsert(new[] { itemType });

		var returned = await this.context.ItemTypes.SingleAsync();

		returned.WithDeepEqual(itemType)
			.IgnoreProperty<ItemType>(c=>c.Id)
			.SkipDefault<ItemType>()
			.Assert();
	}

	private ItemTypeRepository repo;
	private AnalysisContext context;
}