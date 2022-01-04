using Tradie.Common.RawModels;

namespace Tradie.Analyzer;

/// <summary>
/// Analyzes raw items, returning the analyzed results for all items.
/// </summary>
public interface IStashTabAnalyzer {
	/// <summary>
	/// Analyzes the given stash tab, returning a version with each item in the tab analyzed as well.
	/// </summary>
	Task<AnalyzedStashTab> AnalyzeTab(RawStashTab tab);
}

/// <summary>
/// Basic IStashAnalyzer implementation that analyzes all items within the tab using a collection of IItemAnalyzers.
/// </summary>
public class StashTabAnalyzer : IStashTabAnalyzer {
	public StashTabAnalyzer(IItemAnalyzer[] analyzers) {
		this._analyzers = analyzers;
	}
	
	public async Task<AnalyzedStashTab> AnalyzeTab(RawStashTab tab) {
		if(tab.Items.Length == 0) {
			return new AnalyzedStashTab(tab.Id, Array.Empty<ItemAnalysis>());
		}
		
		var items = tab.Items!.Select(c => new AnalyzedItem(c)).ToArray();
		
		await Parallel.ForEachAsync(this._analyzers, (c, token) => new ValueTask(c.AnalyzeItems(items)));

		return new AnalyzedStashTab(tab.Id, items.Select(c=>c.Analysis).ToArray());
	}

	private readonly IItemAnalyzer[] _analyzers;
}