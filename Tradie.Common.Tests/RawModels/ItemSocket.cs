using DeepEqual.Syntax;
using Tradie.Common.RawModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serializer = SpanJson.JsonSerializer.Generic.Utf16;

namespace Tradie.Common.Tests.RawModels;

[TestClass]
public class ItemSocketTest {

	[TestMethod]
	public void TestFromJson() {
		string json  = 
@"[{
	""group"": 0,
	""attr"": ""D"",
	""sColour"": ""G""
},
{
	""group"": 1,
	""attr"": ""D"",
	""sColour"": ""G""
},
{
	""group"": 2,
	""attr"": ""D"",
	""sColour"": ""G""
}
]";
		var expected = new ItemSocket[] {
			new ItemSocket(0, "D", "G"),
			new ItemSocket(1, "D", "G"),
			new ItemSocket(2, "D", "G")
		};
		var actual = Serializer.Deserialize<ItemSocket[]>(json);
		actual.ShouldDeepEqual(expected);
	}
}