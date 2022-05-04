using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common.RawModels;
using Tradie.TestUtils;
using static Tradie.TestUtils.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers; 

[TestClass]
public class ItemTypeAnalyzerTest : TestBase {
	[TestMethod]
	public async Task TestMissing_NoRequirements() {
		var item = await ItemUtils.ReadTestItem("jewel");
		
		this._repo.Setup(c => c.LoadByNames(new[] { item.BaseType }, CancellationToken.None))
			.ReturnsAsync(new ItemType[] { });

		this._repo.Setup(c => c.Insert(new[] {
			new ItemType() {
				Category = "jewels",
				Height = 1,
				Width = 1,
				Name = "Cobalt Jewel",
				Subcategory = null,
				Requirements = new Requirements(),
			}
		}.DeepMatcher<IEnumerable<ItemType>>(), CancellationToken.None))
			.Callback((IEnumerable<ItemType> args, CancellationToken cancellationToken) => Task.FromResult(args.Single().Id = 36))
			.Returns(Task.CompletedTask);
		
		var logger = CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, this._repo.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
		
		var itemTypeId = ((ItemTypeAnalysis)analyzedItem.Analysis[ItemTypeAnalyzer.Id]!).ItemTypeId; 
		Assert.AreEqual(36, itemTypeId);
	}
	
	[TestMethod]
	public async Task TestMissing_WithRequirements() {
		var item = await ItemUtils.ReadTestItem("boots");
		
		this._repo.Setup(c => c.LoadByNames(new[] { item.BaseType }, CancellationToken.None))
			.ReturnsAsync(new ItemType[] { });

		this._repo.Setup(c => c.Insert(new[] {
			new ItemType() {
				Category = "armour",
				Height = 2,
				Width = 2,
				Name = "Hydrascale Boots",
				Subcategory = "boots",
				Requirements = new Requirements() {
					Level = 60,
					Str = 56,
					Dex = 56,
				}
			}
		}.DeepMatcher<IEnumerable<ItemType>>(), CancellationToken.None))
			.Returns(Task.CompletedTask);
		
		var logger = CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, this._repo.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
	}
	
	[TestMethod]
	public async Task TestExisting() {
		var item = await ItemUtils.ReadTestItem("jewel");
		
		this._repo.Setup(c => c.LoadByNames(new[] { item.BaseType }, CancellationToken.None))
			.ReturnsAsync(new[] {
				new ItemType() {
					Category = "jewels",
					Height = 1,
					Width = 1,
					Name = "Cobalt Jewel",
					Subcategory = null,
					Requirements = new Requirements(),
				}
			});
		
		var logger = CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, this._repo.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
	}

	private readonly Mock<IItemTypeRepository> _repo = null!;
}