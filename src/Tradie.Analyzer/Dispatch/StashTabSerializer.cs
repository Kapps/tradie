using MessagePack;
using System.Buffers;

namespace Tradie.Analyzer.Dispatch;

/// <summary>
/// Supports serialization of fully analyzed stash tabs and the items within them.
/// </summary>
public interface IStashTabSerializer {
	/// <summary>
	/// Serializes this stash tab, streaming the serialization to the given output stream.
	/// </summary>
	Task Serialize(AnalyzedStashTab stashTab, Stream outputStream, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deserializes a single stash tab from the input stream.
	/// This method may be called multiple times if the stream contains multiple tabs.
	/// </summary>
	ValueTask<AnalyzedStashTab> Deserialize(Stream inputStream, CancellationToken cancellationToken = default);
}

public class MessagePackedStashTabSerializer : IStashTabSerializer {
	public static MessagePackSerializerOptions SerializationOptions => MessagePackSerializerOptions.Standard
		.WithCompression(MessagePackCompression.Lz4BlockArray);
	
	public Task Serialize(AnalyzedStashTab stashTab, Stream outputStream, CancellationToken cancellationToken = default) {
		return MessagePackSerializer.SerializeAsync(outputStream, stashTab, SerializationOptions, cancellationToken);
	}

	public ValueTask<AnalyzedStashTab> Deserialize(Stream inputStream, CancellationToken cancellationToken = default) {
		return MessagePackSerializer.DeserializeAsync<AnalyzedStashTab>(inputStream, SerializationOptions, cancellationToken);
	}
}

/*/// <summary>
/// An IStashSerializer that serializes analyzed tabs into an efficient binary format.
/// </summary>
public class BinaryStashTabSerializer : IStashTabSerializer {
	public byte[] Serialize(AnalyzedStashTab stashTab) { 
		// These are small payloads so serializing directly to a byte[] is okay.
		using var ms = new MemoryStream();
		using var writer = new BinaryWriter(ms);
		writer.Write(stashTab.StashTabId);
		writer.Write(stashTab.Items.Length);
		foreach(var item in stashTab.Items) {
			writer.Write(item.RawItem.Id);
			
			var props = item.Analysis.Properties;
			writer.Write(props.Count());
			foreach(var prop in props) {
				writer.Write(prop.Key.ToByteArray());
				prop.Value.Serialize(writer);
			}
		}

		return ms.ToArray();
	}
}*/