using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common.RawModels;
using Tradie.TestUtils;
using static Tradie.TestUtils.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers; 

[TestClass]
public class ItemTypeAnalyzerTest : TestBase {
	[TestMethod]
	public async Task TestMissing() {
		var item = await ItemUtils.ReadTestItem("jewel");
		var converted = new ItemType() {Name = "Cobalt Jewel"};

		this._converter.Setup(c => c.ConvertFromRaw(item))
			.Returns(converted);
		this._repo.Setup(c => c.LoadByNames(new[] { item.BaseType }, CancellationToken.None))
			.ReturnsAsync(Array.Empty<ItemType>());
		this._repo.Setup(c => c.Insert(new[] {
			converted
		}.DeepMatcher<IEnumerable<ItemType>>(), CancellationToken.None))
			.Callback((IEnumerable<ItemType> args, CancellationToken cancellationToken) => Task.FromResult(args.Single().Id = 36))
			.Returns(Task.CompletedTask);
		
		var logger = CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, this._repo.Object, this._converter.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
		
		var itemTypeId = ((ItemTypeAnalysis)analyzedItem.Analysis[ItemTypeAnalyzer.Id]!).ItemTypeId; 
		Assert.AreEqual(36, itemTypeId);
	}

	[TestMethod]
	public async Task TestUpdate() {
		var item = await ItemUtils.ReadTestItem("boots");
		var incoming = new ItemType() {
			Name = "Hydrascale Boots",
			IconUrl = "about:blank"
		};
		var existing = new ItemType() {
			Name = "Hydrascale Boots"
		};

		this._converter.Setup(c => c.ConvertFromRaw(item))
			.Returns(incoming);
		this._converter.Setup(c => c.MergeFrom(existing, incoming))
			.Callback<ItemType, ItemType>((existing, incoming) => { existing.IconUrl = incoming.IconUrl; })
			.Returns(existing);
		this._converter.Setup(c => c.RequiresUpdate(existing, incoming)).Returns(true);

		this._repo.Setup(c => c.LoadByNames(new[] { item.BaseType }, CancellationToken.None))
			.ReturnsAsync(new[] {
				existing
			});
		this._repo.Setup(c => c.Update(
			It.Is<IEnumerable<ItemType>>(c => c.Single().IconUrl == "about:blank"),
			CancellationToken.None
		)).Returns(Task.CompletedTask);
		
		var logger = CreateLogger<ItemTypeAnalyzer>();
		var analyzer = new ItemTypeAnalyzer(logger, this._repo.Object, this._converter.Object);

		var analyzedItem = new AnalyzedItem(item);
		await analyzer.AnalyzeItems(new[] {analyzedItem});
	}

	private readonly Mock<IItemTypeRepository> _repo = null!;
	private readonly Mock<IPersistentEntityConverter<ItemType>> _converter = null!;
}