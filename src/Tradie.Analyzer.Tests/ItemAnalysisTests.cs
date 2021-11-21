using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tradie.Analyzer.Analyzers;

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
		
		analysis.Properties.WithDeepEqual(expected.Select(c=>(TestProperties)c.props).ToArray())
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

	[TestMethod]
	public void TestItemAnalysis_Serialize() {
		using var ms = new MemoryStream();
		using var writer = new BinaryWriter(ms);
		
		var analyzerId = Guid.NewGuid();
		var analysis = new ItemAnalysis();
		analysis.PushAnalysis(analyzerId, new TestProperties() {
			Foo = 3
		});
		
		analysis.Serialize(writer);

		ms.Position = 0;
		using var reader = new BinaryReader(ms);
		Assert.AreEqual(1, reader.ReadInt32()); // Count
		Assert.AreEqual(analyzerId, new Guid(reader.ReadBytes(16))); // Analyzer ID
		var deserializedProps = new TestProperties() {
			Foo = reader.ReadInt32() // AnalyzedProperties Serialized
		};
		Assert.AreEqual(analysis[analyzerId], deserializedProps);
	}

	private struct TestProperties : IAnalyzedProperties {
		public int Foo;
		public void Serialize(BinaryWriter writer) {
			writer.Write(this.Foo);
		}
	}
}