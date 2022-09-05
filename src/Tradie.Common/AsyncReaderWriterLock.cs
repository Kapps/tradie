namespace Tradie.Common;

/// <summary>
/// A ReaderWriterLock that allows async access, operating as a spin-lock with yielding tasks while blocked.
/// Optimized for reads with no blocks.
/// </summary>
public class AsyncReaderWriterLock {
	public async Task AcquireWriteLock() {
		while (true) {
			if (Interlocked.CompareExchange(ref this._writing, 1, 0) == 0) {
				while (this._readerCount > 0) {
					await Task.Yield();
				}
				return;
			}
			await Task.Yield();
		}
	}
		
	public void ReleaseWriteLock() {
		Interlocked.Exchange(ref this._writing, 0);
	}
	
	public async Task AcquireReadLock() {
		while (true) {
			// If we're not writing, increment reader count to signal we have one going.
			if(this._writing != 0) {
				await Task.Yield();
				continue;
			}
			if (Interlocked.Increment(ref this._readerCount) > 0) {
				// But we could have started writing in the time it took to increment, so decrement and try again if so.
				// If we didn't, we're good.
				if(this._writing == 0) {
					return;
				}
			}
			Interlocked.Decrement(ref this._readerCount);
			await Task.Yield();
		}
	}
	
	public void ReleaseReadLock() {
		Interlocked.Decrement(ref this._readerCount);
	}

	private int _writing = 0;
	private int _readerCount = 0;
}