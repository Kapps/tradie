using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Transactions;
using Tradie.Analyzer;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.ItemLog;

namespace Tradie.ItemLogBuilder.Postgres; 

/// <summary>
/// An ItemLogBuilder that stores stash tabs in a table with JSON columns for entries. 
/// </summary>,
public class PostgresLogBuilder : IItemLogBuilder, IAsyncDisposable {
	public PostgresLogBuilder(
			ILogger<IItemLogBuilder> logger,
			ILoggedTabRepository tabRepository,
			AnalysisContext context
		) {
		this._logger = logger;
		this._tabRepository = tabRepository;
		this._context = context;
	}

	public string Name => "Postgres";

	public async Task AppendEntries(IAsyncEnumerable<LogRecord> stashTabs, CancellationToken cancellationToken = default) {
		var visitedTabs = new HashSet<string>();
		var dedupedTabs = stashTabs
			.Reverse() // Take the _latest_ version of each duplicate stash tab; PG can't handle duplicates 
			.Where(c => visitedTabs.Add(c.StashTab.StashTabId));
		
		await using var tx = await this._context.Database.BeginTransactionAsync(cancellationToken);

		var insertedCount = await this._tabRepository.LogTabs(dedupedTabs.Select(c=>c.StashTab), cancellationToken)
			.CountAsync(cancellationToken);
		
		await tx.CommitAsync(cancellationToken);

		this._logger.LogInformation("Appended {InsertedTabCount} tabs to the item log", insertedCount);
		/*var visitedTabs = new HashSet<string>();
		var allRecords = await stashTabs
			.Reverse() // Take the _latest_ version of each duplicate stash tab. 
			.Where(c=>visitedTabs.Add(c.StashTab.StashTabId)) 
			.ToDictionaryAsync(c => c.StashTab.StashTabId, cancellationToken);

		if(!allRecords.Any()) {
			// Let's not bother creating temp tables and such for nothing.
			return;
		}
		
		await using var tx = await this._context.Database.BeginTransactionAsync(cancellationToken);

		var insertedTabs = await _tabRepository.LogTabs(allRecords.Select(c=>c.Value.StashTab).ToAsyncEnumerable(), cancellationToken)
			.ToDictionaryAsync(c => c.RawId, cancellationToken);
		
		this._logger.LogInformation("Appended {insertedTabCount} tabs to the item log.", insertedTabs.Count);

		// Serialize JSON as a dictionary rather than the collection, so that SpanJson handles it properly and not as KVP array.
		var mappedItems = allRecords.Values
			.SelectMany(c => c.StashTab.Items.Select(d =>
				new LoggedItem(
					d.ItemId,
					insertedTabs[c.StashTab.StashTabId].Id,
					SpanJson.JsonSerializer.Generic.Utf8.Serialize(new Dictionary<ushort, IAnalyzedProperties>(d.Properties))
				)))
			.ToAsyncEnumerable();

		var insertedItemCount = await this._itemRepository.LogItems(mappedItems, cancellationToken)
			.CountAsync(cancellationToken);

		await tx.CommitAsync(cancellationToken);
		
		this._logger.LogInformation("Appended {insertedItemCount} items to the item log.", insertedItemCount);*/
	}

	public async ValueTask DisposeAsync() {
		await this._tabRepository.DisposeAsync();
		await this._context.DisposeAsync();
	}

	private readonly ILogger<IItemLogBuilder> _logger;
	private readonly ILoggedTabRepository _tabRepository;
	private readonly AnalysisContext _context;
}