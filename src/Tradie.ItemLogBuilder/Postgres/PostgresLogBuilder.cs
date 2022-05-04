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
			ILogger<PostgresLogBuilder> logger,
			ILoggedTabRepository tabRepository,
			ILoggedItemRepository itemRepository,
			AnalysisContext context
		) {
		this._logger = logger;
		this._tabRepository = tabRepository;
		this._itemRepository = itemRepository; 
		this._context = context;
	}

	public string Name => "Postgres";

	public async Task AppendEntries(IAsyncEnumerable<LogRecord> stashTabs, CancellationToken cancellationToken = default) {
		var visitedTabs = new HashSet<string>();
		var dedupedTabs = (await stashTabs // Reverse converts to an array anyways, so avoid double async.
				.ToArrayAsync(cancellationToken))
			.Reverse() // Take the _latest_ version of each duplicate stash tab; PG can't handle duplicates 
			.Where(c => visitedTabs.Add(c.StashTab.StashTabId))
			.ToDictionary(c => c.StashTab.StashTabId);

		if(dedupedTabs.Count == 0) {
			return;
		}
		
		await using var tx = await this._context.Database.BeginTransactionAsync(cancellationToken);

		var insertedTabs = await this._tabRepository
			.LogTabs(dedupedTabs.Select(c => c.Value.StashTab).ToAsyncEnumerable(), cancellationToken)
			.ToDictionaryAsync(c => c.RawStashTabId, cancellationToken);
		

		this._logger.LogInformation("Appended {InsertedTabCount} tabs to the item log", insertedTabs.Count);
		
		
		// Serialize JSON as a dictionary rather than the collection, so that SpanJson handles it properly and not as KVP array.
		var mappedItems = dedupedTabs.Values
			.SelectMany(c => c.StashTab.Items.Select(d =>
				new LoggedItem(
					d.ItemId,
					insertedTabs[c.StashTab.StashTabId].StashTabId,
					new AnalyzedPropertyCollection(d.ToDictionary())
				)))
			.ToAsyncEnumerable();

		var insertedItemCount = await this._itemRepository.LogItems(insertedTabs.Values.ToArray(), mappedItems, cancellationToken)
			.CountAsync(cancellationToken);

		await tx.CommitAsync(cancellationToken);
		
		this._logger.LogInformation("Appended {insertedItemCount} items to the item log.", insertedItemCount);
	}

	public async ValueTask DisposeAsync() {
		await this._tabRepository.DisposeAsync();
		await this._context.DisposeAsync();
	}

	private readonly ILogger<PostgresLogBuilder> _logger;
	private readonly ILoggedTabRepository _tabRepository;
	private readonly ILoggedItemRepository _itemRepository;
	private readonly AnalysisContext _context;
}