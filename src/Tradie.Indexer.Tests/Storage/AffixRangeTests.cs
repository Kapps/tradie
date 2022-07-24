using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tradie.Indexer.Storage;
using Tradie.TestUtils;

namespace Tradie.Indexer.Tests.Storage;

[TestClass]
public class AffixRangeTests : TestBase {
	[TestMethod]
	public void TestAffixRange() {
		var range = new AffixRange(12);
		Assert.AreEqual(12UL, range.ModHash);
		Assert.AreEqual(FloatRange.NaN, range.Explicit);
		Assert.AreEqual(FloatRange.NaN, range.Implicit);
		Assert.AreEqual(FloatRange.NaN, range.Total);
		
		Assert.IsFalse(range.Explicit.Expand(FloatRange.NaN));
		Assert.AreEqual(FloatRange.NaN, range.Explicit);
		
		Assert.IsTrue(range.Explicit.Expand(new FloatRange(12, 24)));
		Assert.AreEqual(range.Explicit, new FloatRange(12, 24));
		
		Assert.IsFalse(range.Explicit.Expand(FloatRange.NaN));
		Assert.AreEqual(range.Explicit, new FloatRange(12, 24));

		Assert.IsFalse(range.Explicit.Expand(new FloatRange(13, 23)));
		Assert.AreEqual(range.Explicit, new FloatRange(12, 24));
		
		Assert.IsFalse(range.Explicit.Expand(new FloatRange(12, 24)));
		Assert.AreEqual(range.Explicit, new FloatRange(12, 24));

		Assert.IsTrue(range.Explicit.Expand(new FloatRange(12, 25)));
		Assert.AreEqual(range.Explicit, new FloatRange(12, 25));
		
		Assert.IsTrue(range.Explicit.Expand(new FloatRange(11, 26)));
		Assert.AreEqual(range.Explicit, new FloatRange(11, 26));
	}
}