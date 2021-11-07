using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tradie.Common.RawModels;
using Serializer = SpanJson.JsonSerializer.Generic.Utf16;

namespace Tradie.Common.Tests.RawModels; 

public class ExtendedItemPropertiesTest {

	[TestMethod]
	public void FromJson() {
		string json = @"{
	        ""category"": ""accessories"",
	        ""subcategories"": [
	            ""amulet""
	        ],
	        ""prefixes"": 1,
	        ""suffixes"": 3
	    }";
		
		var expected = new ExtendedItemProperties("accessories", new[] {"amulet"}, 1, 3);
		var actual = Serializer.Deserialize<ExtendedItemProperties>(json);
		
		actual.WithDeepEqual(expected).SkipDefault<ExtendedItemProperties>().Assert();
	}
	
	
}