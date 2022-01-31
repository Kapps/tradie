using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tradie.Analyzer;
using Tradie.Analyzer.Entities;
using Tradie.Common;
using Tradie.ItemLog;

namespace Tradie.ItemLogBuilder.Postgres; 

/// <summary>
/// An ItemLogBuilder that stores stash tabs in a table with JSON columns for entries. 
/// </summary>,
public class PostgresLogBuilder : IItemLogBuilder, IAsyncDisposable {
	public PostgresLogBuilder(
			ILogger<PostgresLogBuilder> logger,
			ILoggedItemRepository itemRepository,
			ILoggedTabRepository tabRepository
		) {
		this._logger = logger;
		this._itemRepository = itemRepository;
		this._tabRepository = tabRepository;
	}

	public Guid Id => new Guid("4EE76D12-27FF-41D6-A5F3-738FAA42147F");

	public async Task AppendEntries(IAsyncEnumerable<LogRecord> stashTabs, CancellationToken cancellationToken = default) {
	var allRecords = batch
			.Reverse()	 // Take the last 
			.DistinctBy(c=>c.StashTab.StashTabId)
			.ToDictionary(c => c.StashTab.StashTabId);
		var insertedTabs = await _tabRepository.LogTabs(allRecords.Select(c=>c.Value.StashTab).ToAsyncEnumerable(), cancellationToken)
			.ToDictionaryAsync(c => c.RawId, cancellationToken);
		
		this._logger.LogInformation("Appended {insertedTabCount} tabs to the item log.", insertedTabs.Count);

		var mappedItems = allRecords.Values
			.SelectMany(c => c.StashTab.Items.Select(d =>
				new LoggedItem(
					d.ItemId,
					insertedTabs[c.StashTab.StashTabId].Id,
					SpanJson.JsonSerializer.Generic.Utf8.Serialize(d.Properties)
				)))
			.ToAsyncEnumerable();

		var insertedItemCount = await this._itemRepository.LogItems(mappedItems, cancellationToken)
			.CountAsync(cancellationToken);
		
		this._logger.LogInformation("Appended {insertedItemCount} items to the item log.", insertedItemCount);
	}

	public async ValueTask DisposeAsync() {
		await this._itemRepository.DisposeAsync();
		await this._tabRepository.DisposeAsync();
	}

	private readonly ILogger<PostgresLogBuilder> _logger;
	private readonly ILoggedItemRepository _itemRepository;
	private readonly ILoggedTabRepository _tabRepository;
}