namespace Tradie.Analyzer.Analyzers;

/// <summary>
/// Registers hardcoded IDs for known analyzers, allowing efficient serialization by using small integers.
/// If an analyzer is no longer used, its ID must remain until all previous references are no longer in use.
/// </summary>
public static class KnownAnalyzers {
	public const int ItemType = 1;
	public const int Modifiers = 2;
	public const int TradeAttributes = 3;
	public const int ItemDetails = 4;

	/// <summary>
	/// The maximum value of a known analyzer.
	/// </summary>
	public const int Max = ItemDetails;

	/// <summary>
	/// Returns the cached type associated with a given analyzer id.
	/// </summary>
	public static Type GetTypeForAnalyzer(ushort analyzerId) {
		if(analyzerId == 0 || analyzerId > AnalyzerTypes.Length)
			throw new ArgumentOutOfRangeException(nameof(analyzerId));
		return AnalyzerTypes[analyzerId]!;
	}

	public static ushort GetAnalyzerIdForType(Type analyzedPropertyType) {
		var res = typeToAnalyzerId[analyzedPropertyType];
		if(res == 0) {
			throw new KeyNotFoundException(analyzedPropertyType.FullName);
		}

		return res;
	}

	private static readonly Type?[] AnalyzerTypes = new[] {
		null,
		typeof(ItemTypeAnalysis),
		typeof(ItemAffixesAnalysis),
		typeof(TradeListingAnalysis),
		typeof(ItemDetailsAnalysis)
	};

	private static readonly Dictionary<Type, ushort> typeToAnalyzerId = new() {
		{typeof(ItemTypeAnalysis), 1},
		{typeof(ItemAffixesAnalysis), 2},
		{typeof(TradeListingAnalysis), 3},
		{typeof(ItemDetailsAnalysis), 4}
	};
}