using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Tradie.Common.Storage;

/*public interface IBlobStore {
	/// <summary>
	/// Loads the blob with the given key.
	/// </summary>
	Task<IBlob> Load(string key);
}

public class BlobStore : IBlobStore {
	private static readonly Regex RecordRefRegex =
		new Regex(@"^\{""blobKey"": ""(.+?)""\}$", RegexOptions.Compiled | RegexOptions.Singleline);

	public static bool TryGetRecordKey(string contents, [NotNullWhen(true)] out string? key) {
		var matches = RecordRefRegex.Match(contents);
		if(!matches.Success) {
			key = null;
			return false;
		}

		key = matches.Groups[1].Value;
		return true;
	}
}*/