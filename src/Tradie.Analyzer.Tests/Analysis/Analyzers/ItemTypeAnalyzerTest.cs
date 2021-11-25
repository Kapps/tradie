using DeepEqual.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tradie.Analyzer.Analysis;
using Tradie.Analyzer.Analysis.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Tests.Analyzers; 

[TestClass]
public class ItemTypeAnalyzerTest {
	[TestMethod]
	public async Task TestMissing_NoRequirements() {
		var item = await TestUtils.ReadTestItem("jewel");
		
		var repo = new Mock<IItemTypeRepository>(MockBehavior.Strict);
		repo.Setup(c => c.LoadByNames(item.BaseType))
			.ReturnsAsync(new ItemType[] { });

		repo.Setup(c => c.Insert(TestUtils.DeepEquals<IEnumerable<ItemType>>(new[] {
			new ItemType() {
				Category = "jewels",
				Height = 1,
				Width = 1,
				Name = "Cobalt Jewel",
				Subcategory = null,
				Requirements = new Requirements(),
			}
		}))).Returns(Task.CompletedTask);
		
		var logger = TestUtils.CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, repo.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
		
		var itemType = ((ItemTypeAnalysis)analyzedItem.Analysis[ItemTypeAnalyzer.Id]).ItemType; 
		Assert.IsTrue(itemType.Name == "Cobalt Jewel");
		
		repo.VerifyAll();
	}
	
	[TestMethod]
	public async Task TestMissing_WithRequirements() {
		var item = await TestUtils.ReadTestItem("boots");
		
		var repo = new Mock<IItemTypeRepository>(MockBehavior.Strict);
		repo.Setup(c => c.LoadByNames(item.BaseType))
			.ReturnsAsync(new ItemType[] { });

		repo.Setup(c => c.Insert(TestUtils.DeepEquals<IEnumerable<ItemType>>(new[] {
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
		}))).Returns(Task.CompletedTask);
		
		var logger = TestUtils.CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, repo.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
		
		repo.VerifyAll();
	}
	
	[TestMethod]
	public async Task TestExisting() {
		var item = await TestUtils.ReadTestItem("jewel");
		
		var repo = new Mock<IItemTypeRepository>(MockBehavior.Strict);
		repo.Setup(c => c.LoadByNames(item.BaseType))
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
		
		var logger = TestUtils.CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, repo.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
		
		repo.VerifyAll();
	}
	
}