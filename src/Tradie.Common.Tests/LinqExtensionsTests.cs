using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

	[TestMethod]
	public void TestUnorderedSequenceEquals_BothEmpty() {
		var c1 = Array.Empty<int>();
		var c2 = Array.Empty<int>();
		
		Assert.IsTrue(c1.UnorderedSequenceEquals(c2));
		Assert.IsTrue(c2.UnorderedSequenceEquals(c1));
	}
	
	[TestMethod]
	public void TestUnorderedSequenceEquals_EmptyNull() {
		int[] c1 = Array.Empty<int>();
		int[] c2 = null;
		
		Assert.IsTrue(c1.UnorderedSequenceEquals(c2));
		Assert.IsTrue(c2.UnorderedSequenceEquals(c1));
	}

	[TestMethod]
	public void TestUnorderedSequenceEquals_EmptyValid() {
		int[] c1 = new[] {1, 2, 3};
		int[] c2 = Array.Empty<int>();
		
		Assert.IsFalse(c1.UnorderedSequenceEquals(c2));
		Assert.IsFalse(c2.UnorderedSequenceEquals(c1));
	}
	
	[TestMethod]
	public void TestUnorderedSequenceEquals_EqualElements() {
		int[] c1 = new[] {1, 2, 3};
		int[] c2 = new[] { 1, 2, 3};
		
		Assert.IsTrue(c1.UnorderedSequenceEquals(c2));
		Assert.IsTrue(c2.UnorderedSequenceEquals(c1));
	}
	
	[TestMethod]
	public void TestUnorderedSequenceEquals_UnequalElements() {
		int[] c1 = new[] {1, 2, 3};
		int[] c2 = new[] { 1, 4, 3};
		
		Assert.IsFalse(c1.UnorderedSequenceEquals(c2));
		Assert.IsFalse(c2.UnorderedSequenceEquals(c1));
	}
	
	[TestMethod]
	public void TestUnorderedSequenceEquals_EqualUnsortedElements() {
		int[] c1 = new[] {1, 2, 3};
		int[] c2 = new[] { 2, 1, 3};
		
		Assert.IsTrue(c1.UnorderedSequenceEquals(c2));
		Assert.IsTrue(c2.UnorderedSequenceEquals(c1));
	}
}