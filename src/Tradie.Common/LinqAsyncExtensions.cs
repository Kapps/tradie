using System.Runtime.CompilerServices;

namespace Tradie.Common {
	/// <summary>
	/// Extension Linq methods for asynchronous enumerables.
	/// </summary>
	public static class LinqAsyncExtensions {
		/// <summary>
		/// Returns the source enumerable in chunks of batchSize.
		/// Each batch reads from the source enumerable, and the enumerable is iterated over only once.
		/// When a batch is returned, it must be fully read prior to the next batch being requested.
		/// This means attempting to use aggregation functions over the resulting array will cause an InvalidOperationException.
		/// If the source is empty, a single batch with no elements will be returned.
		/// </summary>
		public static async IAsyncEnumerable<IAsyncEnumerable<T>> BatchByAsync<T>(this IAsyncEnumerable<T> src, int batchSize, [EnumeratorCancellation] CancellationToken cancellationToken) {
			bool eof = false;
			bool ready = true;
			await using var enumerator = src.GetAsyncEnumerator(cancellationToken);

			if(!await enumerator.MoveNextAsync()) {
				yield break;
			}
			
			async IAsyncEnumerable<T> Batcher() {
				for(int i = 0; i < batchSize; i++) {
					yield return enumerator.Current;

					if(!await enumerator.MoveNextAsync()) {
						eof = true;
						break;
					}
				}

				ready = true;
			}

			while(!eof) {
				if(!ready) {
					throw new InvalidOperationException(
						"Unable to iterate to next batch prior to processing previous batch.");
				}
				ready = false;
				yield return Batcher();
			}
		}

		/// <summary>
		/// Returns an IAsyncEnumerable that wraps the source enumerable, returning entries from it.
		/// If any entry was returned, callback is invoked after iteration with the last entry to be returned as the param.
		/// </summary>
		public static async IAsyncEnumerable<T> WithCompletionCallback<T>(this IAsyncEnumerable<T> src, Func<T, Task> callback) {
			T? lastEntry = default;
			bool any = false;
			
			await foreach(var entry in src) {
				lastEntry = entry;
				any = true;
				yield return entry;
			}

			if(any) {
				await callback(lastEntry!);
			}
		}
	}
}