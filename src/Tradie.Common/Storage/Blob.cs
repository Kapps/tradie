namespace Tradie.Common.Storage;

/// <summary>
/// Represents a blob that can be stored in a remote data store.
/// </summary>
public interface IBlob {
	/// <summary>
	/// Returns the contents of this blob.
	/// </summary>
	Task<Memory<byte>> GetBytes();
}