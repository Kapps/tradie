using Tradie.Analyzer;

namespace Tradie.ItemLogBuilder; 

/// <summary>
/// A service that allows building an item log from snapshots of analyzed stash tabs.
/// </summary>
public interface IItemLogBuilder {
	/// <summary>
	/// Appends the following stash tabs to this item log, replacing existing versions of each tab in the log.
	/// </summary>
	Task AppendEntries(IAsyncEnumerable<AnalyzedStashTab> stashTabs);
}