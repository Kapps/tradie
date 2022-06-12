namespace Tradie.Common.Storage;

/// <summary>
/// A blob implementation that directly stores its contents.
/// </summary>
public class RawBlob : IBlob {
	public RawBlob(byte[] content) {
		this._content = content;
	}
	
	public Task<Memory<byte>> GetBytes() {
		return Task.FromResult(this._content.AsMemory());
	}

	private byte[] _content;
}