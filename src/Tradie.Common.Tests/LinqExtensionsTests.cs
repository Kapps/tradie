using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Tradie.Common.Tests; 

[TestClass]
public class LinqExtensionsTests {
	[TestMethod]
	public void TestEmptyWithEmptyEntries() {
		var coll = Enumerable.Empty<int>()
			.ConcatMany();
		Assert.IsTrue(!coll.Any());
	}
	
	[TestMethod]
	public void TestNonEmptyWithEmptyEntries() {
		var coll = new[] { 1 }
			.ConcatMany();
		coll.ToArray().ShouldDeepEqual(new[] {1});
	}
	
	[TestMethod]
	public void TestNonEmptyWithNullEntries() {
		var coll = new[] { 1 }
			.ConcatMany(null, null);
		coll.ToArray().ShouldDeepEqual(new[] { 1 });
	}
	
	[TestMethod]
	public void TestNullWithEmptyEntries() {
		var coll = ((IEnumerable<int>)null)
			.ConcatMany();
		Assert.IsTrue(!coll.Any());
	}
	
	[TestMethod]
	public void TestNullWithNullEntries() {
		var coll = ((IEnumerable<int>)null)
			.ConcatMany(null, null);
		Assert.IsTrue(!coll.Any());
	}
	
	[TestMethod]
	public void TestNonEmptyWithNonEmptyEntries() {
		var coll = new[] { 1 }
			.ConcatMany(null, new[] { 2 }, null);
		coll.ToArray().ShouldDeepEqual(new[] { 1, 2 });
	}
	
	[TestMethod]
	public void TestNullWithNonEmptyEntries() {
		var coll = ((IEnumerable<int>)null)
			.ConcatMany(null, new[] { 2 }, null);
		coll.ToArray().ShouldDeepEqual(new[] { 2 });
	}
}