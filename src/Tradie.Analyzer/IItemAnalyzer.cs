using System;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer;

/// <summary>
/// Base interface for an analyzer that can read raw items and extract information from them for future processing.
/// </summary>
public interface IItemAnalyzer : IAsyncDisposable {
	/// <summary>
    /// Analyzes the list of items, appending the analysis results to any applicable items.
    /// </summary>
	ValueTask AnalyzeItems(AnalyzedItem[] items);
}
