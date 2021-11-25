namespace Tradie.Analyzer.Stores;

/// <summary>
/// Supports serialization of fully analyzed stash tabs and the items within them.
/// </summary>
public interface IStashTabSerializer {
	/// <summary>
	/// Serializes this stash tab, returning the raw bytes.
	/// </summary>
	byte[] Serialize(AnalyzedStashTab stashTab);
}

/// <summary>
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
}