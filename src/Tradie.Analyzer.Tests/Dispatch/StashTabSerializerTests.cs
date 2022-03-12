using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Tests.Analyzers;
using static Tradie.TestUtils.TestUtils;

namespace Tradie.Analyzer.Tests.Dispatch; 

[TestClass]
public class StashTabSerializerTests {
	[TestMethod]
	public async Task TestRoundTrip() {
		var serializer = new MessagePackedStashTabSerializer();
		var analyzerId = (ushort)1;

		var serializeStream = new MemoryStream();
		var rawItem = await ItemUtils.ReadTestItem("boots");
		var analyzedItem = new AnalyzedItem(rawItem);

		analyzedItem.Analysis.PushAnalysis(analyzerId, new ItemTypeAnalysis(3));

		var tabs = new[] {
			new AnalyzedStashTab("foo", "name", null, "acc", "Scourge", "Standard", new[] {
				analyzedItem.Analysis
			}),
			new AnalyzedStashTab("bar", "name", null, "acc", "Scourge", "Standard", new[] {
				analyzedItem.Analysis
			}),
		};

		foreach(var tab in tabs) {
			await serializer.Serialize(tab, serializeStream);
		}

		serializeStream.Position = 0;

		foreach(var tab in tabs) {
			var actual = await serializer.Deserialize(serializeStream);
			actual.WithDeepEqual(tab)
				.SkipDefault<AnalyzedStashTab>()
				.SkipDefault<AnalyzedItem>()
				.Assert();
		}
	}
}