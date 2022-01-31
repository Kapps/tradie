using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tradie.Common.RawModels;

namespace Tradie.Common.Tests.RawModels; 

[TestClass]
public class InfluenceTest {
	[TestMethod]
	public void TestFromJson() {
		string json = @"{
			""hunter"": true,
			""crusader"": true
		}";
		var expected = new Influence(false, true, false, false, true, false);
		var actual = SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Influence>(json);
		actual.ShouldDeepEqual(expected);
	}
}