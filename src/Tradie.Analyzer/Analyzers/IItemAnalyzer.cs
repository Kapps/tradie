using System;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers {
	/// <summary>
	/// Base interface for an analyzer that can read raw items and extract information from them for future processing.
	/// </summary>
    public interface IItemAnalyzer {
	    /// <summary>
	    /// The ID of this analyzer.
	    /// </summary>
	    static Guid Id { get; }
	    /// <summary>
	    /// Analyzes the list of items, appending the analysis results to any applicable items.
	    /// </summary>
	    Task AnalyzeItems(AnalyzedItem[] items);
    }
}

