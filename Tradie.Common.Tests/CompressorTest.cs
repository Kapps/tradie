using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradie.Common.Tests {
	[TestClass]
	public class CompressorTest {
		[TestMethod]
		public void TestEndToEnd() {
			var compressor = new BrotliCompressor();
			var bytes = new byte[16_384_000];
			var rnd = new Random();
			for(int i = 0; i < bytes.Length; i++) {
				bytes[i] = (byte)(i % 10);
			}

			var sw = Stopwatch.StartNew();
			var compressed = compressor.Compress(bytes);
			Assert.IsTrue(compressed.Length < bytes.Length);
			Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms to compress data.");

			sw.Restart();
			var decompressed = compressor.Decompress(compressed);
			Assert.IsTrue(decompressed.SequenceEqual(bytes));
			Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms to decompress.");
		}
	}
}
