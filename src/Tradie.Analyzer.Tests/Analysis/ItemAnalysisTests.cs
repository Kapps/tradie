using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tradie.Analyzer.Analysis;

namespace Tradie.Analyzer.Tests; 

[TestClass]
public class ItemAnalysisTests {
	[TestMethod]
	public void TestItemAnalysis() {
		var analysis = new ItemAnalysis();
		List<dynamic> expected = new();
		for(int i = 0; i < 10; i++) {
			var item = new {
				props = new TestProperties() {
					Foo = i,
				},
				analyzerId = Guid.NewGuid(),
			};
			expected.Add(item);
			analysis.PushAnalysis(item.analyzerId, item.props);
		}

		foreach(var item in expected) {
			Assert.AreEqual(analysis[item.analyzerId], item.props);
		}
		
		analysis.Properties.Select(c=>c.Value).OrderBy(c=>((TestProperties)c).Foo)
			.WithDeepEqual(expected.Select(c=>(TestProperties)c.props).OrderBy(c=>c.Foo).ToArray())
			.Assert();
	}

	[TestMethod]
	public void TestItemAnalysis_DuplicateKey() {
		var analysis = new ItemAnalysis();
		var prop = new TestProperties() {
			Foo = 3,
		};

		var analyzerId = Guid.NewGuid();
		analysis.PushAnalysis(analyzerId, prop);
		Assert.ThrowsException<ArgumentException>(() => analysis.PushAnalysis(analyzerId, prop));
	}

	internal struct TestProperties : IAnalyzedProperties {
		public int Foo;
		public void Serialize(BinaryWriter writer) {
			writer.Write(this.Foo);
		}
	}
}