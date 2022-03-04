#if false
using MongoDB.Driver;
using Tradie.ItemLog;
using Tradie.ItemLogBuilder.Mongo.Models;

namespace Tradie.ItemLogBuilder.Mongo;

public class MongoLogBuilder : IItemLogBuilder {
	public MongoLogBuilder(IMongoDatabase mongoClient) {
		this._tabCollection = mongoClient.GetCollection<LoggedTab>("tabs");
	}

	public string Name { get; } = "Mongo";

	public async Task AppendEntries(IAsyncEnumerable<LogRecord> records,
		CancellationToken cancellationToken = default) {
		var loadedRecords = await records
			.Select(LoggedTab.FromLogRecord)
			.ToArrayAsync(cancellationToken);

		await this._tabCollection.InsertManyAsync(loadedRecords, new InsertManyOptions() {
			IsOrdered = false
		}, cancellationToken);
	}

	private readonly IMongoCollection<LoggedTab> _tabCollection;
}
#endif