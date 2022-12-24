using System.Linq;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.ItemLog;

namespace Tradie.ItemLogBuilder.Elastic.Models;

/// <summary>
/// The ID of the tab within the PoE database.
/// </summary>
public record LoggedTab(
	string StashTabId,
	string? Name,
	string? LastCharacterName,
	string? AccountName,
	string? League,
	string LogOffset,
	string? Kind,
	DateTime LastIndexed,
	LoggedItem[] Items
) {
	public static LoggedTab FromLogRecord(LogRecord record) {
		var tab = record.StashTab;
		var items = tab.Items.Select(c =>
				new LoggedItem(
					c.ItemId, 
					c.Get<ItemTypeAnalysis>(KnownAnalyzers.ItemType),
					c.Get<ItemAffixesAnalysis>(KnownAnalyzers.Modifiers),
					c.Get<ItemDetailsAnalysis>(KnownAnalyzers.ItemDetails),
					c.Get<TradeListingAnalysis>(KnownAnalyzers.TradeAttributes)
				)
			)
			.ToArray();
		return new(
			tab.StashTabId, tab.Name, tab.LastCharacterName, tab.AccountName, tab.League,
			record.Offset.Offset ?? "0", tab.Kind, DateTime.Now, items
		);
	}
}
