using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradie.Common {
	/// <summary>
	/// Provides mechanisms for compressing input and output byte arrays.
	/// </summary>
	public interface ICompressor {
		/// <summary>
		/// Compresses a buffer and returns a new buffer with the fully compressed data.
		/// </summary>
		byte[] Compress(byte[] source);
		/// <summary>
		/// Decompresses a buffer that was compressed via this same compressor.
		/// </summary>
		byte[] Decompress(byte[] source);
	}

	public class BrotliCompressor : ICompressor {
		public byte[] Compress(byte[] source) {
			using var src = new MemoryStream(source);
			using var dest = new MemoryStream();
			using var compressor = new BrotliStream(dest, (CompressionLevel)8);
			src.CopyTo(compressor);
			compressor.Flush();
			return dest.ToArray();
			/*var ms = new MemoryStream();
			int contentsOffset = 0;
			using(var encoder = new BrotliEncoder(8, 16)) {
				byte[] buff = new byte[16384];
				OperationStatus status;
				do {
					var sourceSpan = new ReadOnlySpan<byte>(source, contentsOffset, source.Length - contentsOffset);
					bool finalBlock = source.Length <= buff.Length;
					status = encoder.Compress(sourceSpan, new Span<byte>(buff), out int bytesConsumed, out int bytesWritten, finalBlock);
					if(status == OperationStatus.InvalidData) {
						throw new InvalidDataException();
					}
					if(finalBlock && status != OperationStatus.Done) {
						throw new InvalidDataException("Expected a Done result for a final block.");
					}
					if(status == OperationStatus.Done && !finalBlock) {
						status = encoder.Compress(sourceSpan, new Span<byte>(buff), out bytesConsumed, out bytesWritten, true);
						if(status != OperationStatus.Done) {
							throw new InvalidOperationException();
						}
					}
					ms.Write(buff, 0, bytesWritten);
					contentsOffset += bytesConsumed;
				} while(status != OperationStatus.Done);
			}
			return ms.ToArray();*/
		}

		public byte[] Decompress(byte[] compressedContents) {
			var src = new MemoryStream(compressedContents);
			var dest = new MemoryStream();
			var uncompressor = new BrotliStream(src, CompressionMode.Decompress);
			uncompressor.CopyTo(dest);
			uncompressor.Flush();
			return dest.ToArray();
			/*var ms = new MemoryStream();
			var sourceOffset = 0;
			byte[] buff = new byte[16384];
			using(var decoder = new BrotliDecoder()) {
				var buffSpan = new Span<byte>(buff);
				var sourceSpan = new ReadOnlySpan<byte>(compressedContents, sourceOffset, compressedContents.Length - sourceOffset);

				OperationStatus status;
				do {
					status = decoder.Decompress(sourceSpan, buffSpan, out int bytesConsumed, out int bytesWritten);
					if(status == OperationStatus.InvalidData) {
						throw new InvalidDataException();
					}

					ms.Write(buff, 0, bytesWritten);
					sourceOffset += bytesConsumed;
				} while(status != OperationStatus.Done);
			}
			return ms.GetBuffer();*/
		}
	}
}
