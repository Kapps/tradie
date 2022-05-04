using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Tradie.Analyzer.Analyzers;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests;

[TestClass]
public class AnalyzedPropertyCollectionTests : TestBase {
	[TestMethod]
	public void TestAdd() {
		var collection = new AnalyzedPropertyCollection() {
			{KnownAnalyzers.ItemType, new ItemTypeAnalysis(12)}
		};
		Assert.AreEqual(1, collection.Count);
		Assert.AreEqual(new ItemTypeAnalysis(12), collection.Get<ItemTypeAnalysis>(KnownAnalyzers.ItemType));
	}
	
	[TestMethod]
	public void TestToDictionary() {
		var collection = new AnalyzedPropertyCollection() {
			{KnownAnalyzers.ItemType, new ItemTypeAnalysis(12)}
		};
		collection.ToDictionary().ShouldDeepEqual(new Dictionary<ushort, IAnalyzedProperties>() {
			{ KnownAnalyzers.ItemType, new ItemTypeAnalysis(12) }
		});
	}
	
	[TestMethod]
	public void TestEnumerable() {
		var collection = new AnalyzedPropertyCollection() {
			{KnownAnalyzers.ItemType, new ItemTypeAnalysis(12)},
			{KnownAnalyzers.ItemDetails, new ItemDetailsAnalysis("foo", ItemFlags.Corrupted, InfluenceKind.Crusader, null)}
		};
		
		collection.ToArray().ShouldDeepEqual(new IAnalyzedProperties[] {
			new ItemTypeAnalysis(12),
			new ItemDetailsAnalysis("foo", ItemFlags.Corrupted, InfluenceKind.Crusader, null)
		});
	}
}