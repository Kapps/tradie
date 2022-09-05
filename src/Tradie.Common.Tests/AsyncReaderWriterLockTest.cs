using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tradie.Common.Tests;

[TestClass]
public class AsyncReaderWriterLockTest {

	[TestMethod]
	public async Task TestReadBlocksWrite() {
		var rw = new AsyncReaderWriterLock();
		await rw.AcquireReadLock();

		int writeLockStage = 0;
		_ = Task.Run(() => {
			Interlocked.Increment(ref writeLockStage);
			rw.AcquireWriteLock().Wait();
			Interlocked.Increment(ref writeLockStage);
		});

		
		rw.ReleaseReadLock();
		await Task.Delay(100);
		Assert.AreEqual(2, writeLockStage);
		
		rw.ReleaseWriteLock();
	}
	
	[TestMethod]
	public async Task TestManyReadsBlocksWrite() {
		var rw = new AsyncReaderWriterLock();
		int readerCount = 100;

		for(int i = 0; i < readerCount; i++) {
			await rw.AcquireReadLock();
		}

		int writeLockStage = 0;
		_ = Task.Run(() => {
			Interlocked.Increment(ref writeLockStage);
			rw.AcquireWriteLock().Wait();
			Interlocked.Increment(ref writeLockStage);
		});

		await Task.Delay(100);
		Assert.AreEqual(1, writeLockStage);
		
		for(int i = 0; i < readerCount - 1; i++) {
			_ = Task.Run(() => {
				rw.ReleaseReadLock();
			});
		}
		
		await Task.Delay(100);
		Assert.AreEqual(1, writeLockStage);
		
		_ = Task.Run(() => {
			rw.ReleaseReadLock();
		});
		
		await Task.Delay(100);
		Assert.AreEqual(2, writeLockStage);
		
		rw.ReleaseWriteLock();
	}
	
	[TestMethod]
	public async Task TestWriteBlocksRead() {
		var rw = new AsyncReaderWriterLock();
		await rw.AcquireWriteLock();

		int readLockStage = 0;
		_ = Task.Run(() => {
			Interlocked.Increment(ref readLockStage);
			rw.AcquireReadLock().Wait();
			Interlocked.Increment(ref readLockStage);
		});

		await Task.Delay(100);
		Assert.AreEqual(1, readLockStage);
		
		rw.ReleaseWriteLock();
		await Task.Delay(100);
		Assert.AreEqual(2, readLockStage);
		
		rw.ReleaseReadLock();
	}
}