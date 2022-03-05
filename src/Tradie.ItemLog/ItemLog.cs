using Tradie.Analyzer;

namespace Tradie.ItemLog; 

/// <summary>
/// Represents a snapshot of analyzed tabs at a given point in time that are streamed on demand.
/// </summary>
public interface IItemLog {
	/// <summary>
	/// Returns an enumerable that iterates over the items in this log.
	/// </summary>
	/// <param name="offset">The offset of the first item to begin retrieving.</param>
	/// <param name="cancellationToken">Used to stop iteration or retrieval of additional items.</param>
	IAsyncEnumerable<LogRecord> GetItems(ItemLogOffset offset, CancellationToken cancellationToken);
}

/// <summary>
/// Represents an offset within an ItemLog, with the value being dependent on the type of the item log.
/// An offset is only valid in the type that returned it.
/// </summary>
/// <param name="Offset">The log-dependent formatted offset value, or null to start reading from the oldest available record.</param>
public record struct ItemLogOffset(string? Offset) {
	/// <summary>
	/// Returns an ItemLogOffset that begins from the beginning of the log.
	/// </summary>
	public static readonly ItemLogOffset Start = new(null);
}

/// <summary>
/// Provides data about a single record within an ItemLog.
/// </summary>
/// <param name="Offset">The offset to this item within the log.</param>
/// <param name="StashTab">The stash tab present at this offset.</param>
public readonly record struct LogRecord(ItemLogOffset Offset, AnalyzedStashTab StashTab);