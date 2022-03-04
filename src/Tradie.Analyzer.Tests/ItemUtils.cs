using System.IO;
using System.Threading.Tasks;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Tests {
	public static class ItemUtils {
		/// <summary>
		/// Reads an item from the Analyzer json folder with the given file name (excluding extension).
		/// </summary>
		public static async Task<Item> ReadTestItem(string itemName) {
			string contents = await File.ReadAllTextAsync($"SampleItems/{itemName}.json");
			return SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Item>(contents);
		}
	}
}