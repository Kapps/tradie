using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using Tradie.Analyzer.Stores;
using Tradie.Analyzer.Tests.Analyzers;

namespace Tradie.Analyzer.Tests.Stores; 

[TestClass]
public class StashTabSerializerTests {
	[TestMethod]
	public async Task TestSerialization() {
		var serializer = new BinaryStashTabSerializer();
		var analyzerId = Guid.NewGuid();

		var rawItem = await TestUtils.ReadTestItem("boots");
		var analyzedItem = new AnalyzedItem(rawItem);
		
		analyzedItem.Analysis.PushAnalysis(analyzerId, new ItemAnalysisTests.TestProperties() {
			Foo = 3
		});

		var stashTab = new AnalyzedStashTab("foo", new[] {
			analyzedItem
		});

		byte[] serializedContents = serializer.Serialize(stashTab);
		using var reader = new BinaryReader(new MemoryStream(serializedContents));
		
		Assert.AreEqual("foo", reader.ReadString()); // Tab ID
		Assert.AreEqual(1, reader.ReadInt32()); // Count
		// For each (1) Item
		Assert.AreEqual(rawItem.Id, reader.ReadString()); // Item ID
		Assert.AreEqual(1, reader.ReadInt32()); // Number of properties on the item.
		// For each (1) Property
		Assert.AreEqual(analyzerId, new Guid(reader.ReadBytes(16))); // Analyzer ID
		var deserializedProps = new ItemAnalysisTests.TestProperties() {
			Foo = reader.ReadInt32() // AnalyzedProperties Serialized
		};
		Assert.AreEqual(analyzedItem.Analysis[analyzerId], deserializedProps);
	}
}