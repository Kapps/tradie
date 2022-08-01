using System.Text.RegularExpressions;

namespace Tradie.Common; 

public static class ModifierText {
	/// <summary>
	/// Returns the FNV1A 64-bit hash of the given string, excluding any values.
	/// </summary>
	/// <remarks>
	/// Currently this function ignores all non-ascii-letters to determine which part of a string is relevant.
	/// </remarks>
	public static ulong CalculateValueIndependentHash(string modifierText) {
		if (String.IsNullOrWhiteSpace(modifierText))
			throw new ArgumentException("String to hash must not be null or whitespace.");
		const ulong fnv64Offset = 14695981039346656037;
		const ulong fnv64Prime = 1099511628211;
		ulong hash = fnv64Offset;
		unsafe {
			fixed (void* _ptr = modifierText) {
				byte* ptr = (byte*)_ptr;
				for (int i = 0; i < modifierText.Length * 2; i += 2) {
					// Only care about the ASCII characters as we don't have non-ASCII mods.
					byte b = ptr[i];
					if ((b >= 'a' && b <= 'z') || (b >= 'A' && b <= 'Z')) {
						hash ^= b;
						hash *= fnv64Prime;
					}
				}
			}
		}
		return hash;
	}

	/// <summary>
	/// Returns a value-independent version of this modifier text.
	/// Removes any tags that were part of the item to get a clean string.
	/// </summary>
	public static string MakeValueIndependent(string modifierText) {
		string cleanText = CleanText(modifierText);
		return NumericMatcher.Replace(cleanText, "#");
	}

	/// <summary>
	/// Extracts the scalar value from a modifier text.
	/// If there is only a single number in the text, that number is returned.
	/// Otherwise, the average of all numbers in the modifier text is returned.
	/// </summary>
	public static double ExtractScalar(string modifierText) {
		try {
			string cleanText = CleanText(modifierText);
			var values = NumericMatcher.Match(cleanText);
			if(!values.Success)
				return double.NaN;
			//Console.WriteLine("ExtractScalar: " + modifierText + " -> " + String.Join(", ", values.Captures.Select(c=>c.ValueSpan.ToString())));
			double sum = 0;
			int count = 0;
			for(int i = 2; i < values.Groups.Count; i++) {
				if(!values.Groups[i].Success)
					continue;
				string value = values.Groups[i].ValueSpan.ToString();
				sum += double.Parse(value);
				count++;
			}
			if(count == 0)
				return double.NaN;
			return sum / count;
		} catch(Exception ex) {
			throw new FormatException("Error extracting scalar value from modifier text: " + modifierText, ex);
		}
	}

	private static string CleanText(string text) {
		string clean = text;
		while(true) {
			string next = TagCleaner.Replace(clean, "$1");
			if(next == clean)
				break;
			clean = next;
		}

		return clean;
	}
	
	const string Number = "\\{?-?[0-9]+(?:\\.[0-9]+)?\\}?"; // Matches a number, optionally wrapped in curly braces.

	private static readonly Regex NumericMatcher = new(
		$@"((?:({Number}) to ({Number}))|(?:({Number})-({Number}))|(?:({Number})))",
		RegexOptions.Compiled
	);

	private static readonly Regex TagCleaner = new(
		@"<[^>]*>\{([^\}]*)\}",
		RegexOptions.Compiled
	);
	
	//private static readonly Regex NumericMatcher = new("([\\-0-9\\.])+(?:(?: to )|\\-([\\-0-9\\.]+))?", RegexOptions.Compiled);
}