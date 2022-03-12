using DeepEqual.Syntax;
using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tradie.Analyzer;
using Tradie.Analyzer.Tests.Analyzers;
using static Tradie.TestUtils.TestUtils;

namespace Tradie.Analyzer.Tests; 

[TestClass]
public class ItemAnalysisTests {
	[TestMethod]
	public void TestItemAnalysis() {
		var analysis = new ItemAnalysis("foo");
		List<dynamic> expected = new();
		for(int i = 1; i < 4; i++) {
			var item = new {
				props = new TestProperties() {
					Foo = i,
				},
				analyzerId = (ushort)i
			};
			expected.Add(item);
			analysis.PushAnalysis(item.analyzerId, item.props);
		}

		foreach(var item in expected) {
			Assert.AreEqual(analysis[item.analyzerId], item.props);
		}
		
		analysis.Properties.OrderBy(c=>((TestProperties)c).Foo)
			.WithDeepEqual(expected.Select(c=>(TestProperties)c.props).OrderBy(c=>c.Foo).ToArray())
			.Assert();
	}

	[TestMethod]
	public void TestItemAnalysis_DuplicateKey() {
		var analysis = new ItemAnalysis("foo");
		var prop = new TestProperties() {
			Foo = 3,
		};

		var analyzerId = (ushort)2;
		analysis.PushAnalysis(analyzerId, prop);
		Assert.ThrowsException<ArgumentException>(() => analysis.PushAnalysis(analyzerId, prop));
	}

	[MessagePackObject]
	internal record struct TestProperties(
		[property:Key(1)] int Foo
	) : IAnalyzedProperties;
}