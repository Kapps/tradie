#if false
using MongoDB.Bson.Serialization.Attributes;
using Tradie.Analyzer;
using Tradie.ItemLog;

namespace Tradie.ItemLogBuilder.Mongo.Models;

/// <summary>
/// The ID of the tab within the PoE database.
/// </summary>
public record struct LoggedTab(
	[property: BsonId] string StashTabId,
	[property: BsonIgnoreIfDefault] string? Name,
	[property: BsonIgnoreIfDefault] string? LastCharacterName,
	[property: BsonIgnoreIfDefault] string? AccountName,
	[property: BsonIgnoreIfDefault] string? League,
	[property: BsonRequired] string LogOffset,
	[property: BsonRequired] string Kind,
	[property: BsonIgnoreIfDefault] LoggedItem[] Items
) {
	public static LoggedTab FromLogRecord(LogRecord record) {
		var tab = record.StashTab;
		var items = tab.Items.Select(c =>
				new LoggedItem(c.ItemId, new Dictionary<ushort, IAnalyzedProperties>(c.Properties)))
			.ToArray();
		return new(
			tab.StashTabId, tab.Name, tab.LastCharacterName, tab.AccountName, tab.League,
			record.Offset.Offset, tab.Kind, items
		);
	}
}
#endif