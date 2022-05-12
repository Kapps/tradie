using System.IO.Compression;
using Tradie.Common;

namespace Tradie.Web;
	
public class BrotliCompressionProvider : Grpc.Net.Compression.ICompressionProvider {
	public BrotliCompressionProvider() {
		this._compressor = new BrotliCompressor();
	}
	public Stream CreateCompressionStream(Stream stream, CompressionLevel? compressionLevel) {
		return new BrotliStream(stream, compressionLevel ?? CompressionLevel.Optimal);
	}

	public Stream CreateDecompressionStream(Stream stream) {
		return new BrotliStream(stream, CompressionMode.Decompress);
	}

	public string EncodingName { get; } = "br";

	private BrotliCompressor _compressor;
}