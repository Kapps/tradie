using System;

namespace Tradie.Analyzer.Analyzers;

public class PriceHistoryAnalyzer : IItemAnalyzer {
	public PriceHistoryAnalyzer() {
	}

	public int Order => 1000;

	public ValueTask AnalyzeItems(AnalyzedItem[] items) {
		throw new NotImplementedException();
	}

	public ValueTask DisposeAsync() {
		throw new NotImplementedException();
	}
}