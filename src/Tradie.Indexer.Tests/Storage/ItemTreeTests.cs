using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Storage;

[TestClass]
public class ItemTreeTests : TestBase {

	[TestMethod]
	public void TestEmpty() {
		var tree = new ItemTree();
		
	}
}