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
	/// </summary>
	public static string MakeValueIndependent(string modifierText) {
		return NumericMatcher.Replace(modifierText, "#");
	}

	/// <summary>
	/// Extracts the scalar value from a modifier text.
	/// If there is only a single number in the text, that number is returned.
	/// Otherwise, the average of all numbers in the modifier text is returned.
	/// </summary>
	public static double ExtractScalar(string modifierText) {
		try {
			var values = NumericMatcher.Match(modifierText);
			if(!values.Success)
				return double.NaN;
			//Console.WriteLine("ExtractScalar: " + modifierText + " -> " + String.Join(", ", values.Captures.Select(c=>c.ValueSpan.ToString())));
			double sum = 0;
			int count = 0;
			for(int i = 2; i < values.Groups.Count; i++) {
				if(!values.Groups[i].Success)
					continue;
				sum += double.Parse(values.Groups[i].ValueSpan.ToString());
				count++;
			}
			if(count == 0)
				return double.NaN;
			return sum / count;
		} catch(Exception ex) {
			throw new FormatException("Error extracting scalar value from modifier text: " + modifierText, ex);
		}
	}

	/*private static readonly Regex NumericMatcher = new(
		//"(?:([\\-0-9\\.]+)(?:(?: to )|(?:\\-))([\\-0-9\\.]+))|([\\-0-9\\.]+)",
		@"(?:([\-0-9\.]+)\sto\s([\-0-9\.]+))|(?:([\-0-9\.]+)-([\-0-9\.]+))|([\-0-9\.]+)",
		RegexOptions.Compiled
	);*/
	
	const string Number = "-?[0-9]+(?:\\.[0-9]+)?";

	private static readonly Regex NumericMatcher = new(
		//@"(?:([\-0-9\.]+)\sto\s([\-0-9\.]+))|(?:([\-0-9\.]+)-([\-0-9\.]+))|([\-0-9\.]+)",
		$@"((?:({Number}) to ({Number}))|(?:({Number})-({Number}))|(?:({Number})))",
		RegexOptions.Compiled
	);
	//private static readonly Regex NumericMatcher = new("([\\-0-9\\.])+(?:(?: to )|\\-([\\-0-9\\.]+))?", RegexOptions.Compiled);
}