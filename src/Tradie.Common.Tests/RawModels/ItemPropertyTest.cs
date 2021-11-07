using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tradie.Common.RawModels;
using DeepEqual.Syntax;

namespace Tradie.Common.Tests.RawModels;

[TestClass]
public class ItemPropertyTest {

	[TestMethod]
	public void TestFromJson() {
		string json = "{\"name\":\"Level\",\"values\":[[\"54\",11]],\"displayMode\":12}";
		var expected = new ItemProperty("Level", new ItemPropertyValue[] {
			new ItemPropertyValue("54", 11),
		}, 12);

		var res = SpanJson.JsonSerializer.Generic.Utf16.Deserialize<ItemProperty>(json);
		res.WithDeepEqual(expected).SkipDefault<ItemProperty>().Assert();
	}

	[TestMethod]
	public void TestToJson() {
		var prop = new ItemProperty("Level", new ItemPropertyValue[] {
			new ItemPropertyValue("54", 11),
		}, 12);
		var expectedJson = "{\"name\":\"Level\",\"values\":[[\"54\",11]],\"displayMode\":12}";
		string actualJson = SpanJson.JsonSerializer.Generic.Utf16.Serialize(prop).Replace("\n", "");
		Assert.AreEqual(expectedJson, actualJson);
	}
}
