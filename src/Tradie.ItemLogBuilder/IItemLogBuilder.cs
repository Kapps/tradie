using Tradie.Analyzer;
using Tradie.ItemLog;

namespace Tradie.ItemLogBuilder; 

/// <summary>
/// A service that allows building an item log from snapshots of analyzed stash tabs.
/// </summary>
public interface IItemLogBuilder {
	/// <summary>
	/// A unique ID for this log implementation.
	/// </summary>
	Guid Id { get; }

	/// <summary>
	/// Appends the following records to this item log, replacing existing versions of each tab in the log.
	/// </summary>
	Task AppendEntries(IAsyncEnumerable<LogRecord> records, CancellationToken cancellationToken = default);
}

// Originally going to use below MultiLogBuilder, and still might.
// But currently leaning more towards two different Lambda functions.
/*/// <summary>
/// Provides an IItemLogBuilder implementation that forwards calls to multiple other logs.
/// No attempts at synchronizing failures is provided; if a call fails, it is possible some builders will have already
/// written the contents, while other builders will not get called.
/// </summary>
public class MultiLogBuilder : IItemLogBuilder {
	/// <summary>
	/// Creates a new MultiLogBuilder that dispatches to all of the given builders.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Array is empty.</exception>
	public MultiLogBuilder(IEnumerable<IItemLogBuilder> builders) {
		this._builders = builders.ToArray();
		
		if(this._builders.Length == 0) {
			throw new ArgumentOutOfRangeException("builders.Length");
		}
	}
	
	public async Task AppendEntries(IAsyncEnumerable<AnalyzedStashTab> stashTabs, CancellationToken cancellationToken = default) {
		foreach(var builder in this._builders) {
			await builder.AppendEntries(st)
		}
	}

	private readonly IItemLogBuilder[] _builders;
}*/