#if false
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Core.Compression;
using MongoDB.Driver.Core.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Tradie.Common;
using Tradie.ItemLogBuilder.Mongo;
using Tradie.ItemLogBuilder.Mongo.Models;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests.Mongo;

[TestClass]
public class MongoLogBuilderTests : TestBase {
	private const string CollectionName = "tabs";
	
	protected override void Initialize() {
		var settings = MongoClientSettings.FromConnectionString(TradieConfig.MongoItemLogConnectionString);
		this._mongoClient = new MongoClient(settings);
		var db = this._mongoClient.GetDatabase("tradie");
		db.DropCollection(CollectionName);

		this._logBuilder = new(db);
		this._collection = db.GetCollection<LoggedTab>(CollectionName);
		
	}

	[TestMethod]
	public async Task TestIntegration() {
		var records = new LogRecord[] {
			LogRecordUtils.CreateRecord("tab")
		};
		
		await this._logBuilder.AppendEntries(records.ToAsyncEnumerable());
		
		// Can't load yet due to Mongo serialization issues.
	}

	private MongoLogBuilder _logBuilder = null!;
	private MongoClient _mongoClient = null!;
	private IMongoCollection<LoggedTab> _collection = null!;
	private IClientSessionHandle _session = null!;
}
#endif