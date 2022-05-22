using Tradie.Analyzer.Models;
using Tradie.Common;

namespace Tradie.Analyzer.Analyzers.Conversions;

/// <summary>
/// Handles parsing out modifiers from strings.
/// </summary>
public static class ModParser {
	/// <summary>
	/// Parses the affix details out of a raw text string.
	/// </summary>
	public static Affix ParseAffix(string rawText, ModKind kind) {
		var hash = ModifierText.CalculateValueIndependentHash(rawText);
		var scalar = ModifierText.ExtractScalar(rawText);
		return new Affix(hash, scalar, kind);
	}

	/// <summary>
	/// Extracts all 
	/// </summary>
	/// <param name="texts"></param>
	/// <param name="kind"></param>
	/// <returns></returns>
	public static IEnumerable<Affix> ExtractAffixes(IEnumerable<string> texts, ModKind kind) {
		return texts
			.Where(c => !String.IsNullOrWhiteSpace(c))
			.Select(c => ParseAffix(c, kind));
	}
}