using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tradie.Common.Tests {
	[TestClass]
	public class LinqAsyncExtensionsTests {
		[TestMethod]
		public async Task TestBatchByAsync_BelowBatchSize() {
			var items = new[] { 1, 2, 3 }.ToAsyncEnumerable();
			var enumerator = items.BatchByAsync(5, CancellationToken.None)
				.GetAsyncEnumerator();
			Assert.IsTrue(await enumerator.MoveNextAsync());
			(await enumerator.Current.ToArrayAsync()).ShouldDeepEqual(new[] { 1, 2, 3 });
			Assert.IsFalse(await enumerator.MoveNextAsync());
		}

		[TestMethod]
		public async Task TestBatchByAsync_AboveBatchSize() {
			var items = new[] {1, 2, 3}.ToAsyncEnumerable();
			var enumerator = items.BatchByAsync(2, CancellationToken.None)
				.GetAsyncEnumerator();
			Assert.IsTrue(await enumerator.MoveNextAsync());
			(await enumerator.Current.ToArrayAsync()).ShouldDeepEqual(new[] { 1, 2 });
			Assert.IsTrue(await enumerator.MoveNextAsync());
			(await enumerator.Current.ToArrayAsync()).ShouldDeepEqual(new[] { 3 });
		}
		
		[TestMethod]
		public async Task TestBatchByAsync_EqualToBatchSize() {
			var items = new[] {1, 2, 3}.ToAsyncEnumerable();
			var enumerator = items.BatchByAsync(3, CancellationToken.None)
				.GetAsyncEnumerator();
			Assert.IsTrue(await enumerator.MoveNextAsync());
			(await enumerator.Current.ToArrayAsync()).ShouldDeepEqual(new[] { 1, 2, 3 });
			Assert.IsFalse(await enumerator.MoveNextAsync());
		}

		[TestMethod]
		public async Task TestBatchByAsync_Empty() {
			var items = Array.Empty<int>().ToAsyncEnumerable();
			var enumerator = items.BatchByAsync(5, CancellationToken.None)
				.GetAsyncEnumerator();
			Assert.IsFalse(await enumerator.MoveNextAsync());
		}

		[TestMethod]
		public async Task TestWithCompletionCallback_Empty() {
			var items = Array.Empty<int>().ToAsyncEnumerable();
			bool set = false;
			await items.WithCompletionCallback(_ => Task.FromResult(set = true))
				.ToArrayAsync();
			Assert.IsFalse(set);
		}
		
		[TestMethod]
		public async Task TestWithCompletionCallback_SingleEntry() {
			var items = new[] { 12 }.ToAsyncEnumerable();
			int last = -1;
			await items.WithCompletionCallback(val => Task.FromResult(last = val))
				.ToArrayAsync();
			Assert.AreEqual(12, last);
		}
		
		[TestMethod]
		public async Task TestWithCompletionCallback_Multiple() {
			var items = new[] { 12, 24, 36 }.ToAsyncEnumerable();
			int last = -1;
			await items.WithCompletionCallback(val => Task.FromResult(last = val))
				.ToArrayAsync();
			Assert.AreEqual(36, last);
		}
		
		[TestMethod]
		public async Task TestWithCompletionCallback_ValidateSingleCall() {
			var items = new[] { 12, 24, 36 }.ToAsyncEnumerable();
			int callCount = 0;
			await items.WithCompletionCallback(val => Task.FromResult(callCount++))
				.ToArrayAsync();
			Assert.AreEqual(1, callCount);
		}
	}
}