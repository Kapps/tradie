using DeepEqual.Syntax;
using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests;

[TestClass]
public class PostgresItemLogTests : TestBase {
	protected override void Initialize() {
		var serializer = new MessagePackedStashTabSerializer();
		this._tabs = new LoggedStashTab[] {
			new("public", DateTime.UtcNow, DateTime.UtcNow,  "name1", "lcn1", "an1", "league1", "kind1", Array.Empty<LoggedItem>(), MessagePackSerializer.Serialize(new ItemAnalysis[] {
				new("itemId1", new Dictionary<ushort, IAnalyzedProperties>() {
					{1, new ItemTypeAnalysis(34)}
				})
			}, MessagePackedStashTabSerializer.SerializationOptions)),
			new("id2", DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, "kind2", Array.Empty<LoggedItem>(), null)
		};
		
		this._itemLog = new(this._context);
		
		this._context.LoggedStashTabs.AddRange(this._tabs);
		this._context.SaveChanges();
	}

	[TestMethod]
	public async Task TestGetItems() {
		var res = this._itemLog.GetItems(new ItemLogOffset(null), CancellationToken.None);
		var results = await res.ToArrayAsync();
		
		Assert.AreEqual(2, results.Length);
		// First tab is public with items.
		var fs = this._tabs[0];
		var fr = results[0];
		fr.WithDeepEqual(new LogRecord(fr.Offset, new AnalyzedStashTab(fs.RawId, fs.Name, fs.LastCharacterName, fs.Owner,
			fs.League, fs.Kind, fs.Items.Select(c=>new ItemAnalysis(c.RawId, c.Properties)).ToArray())))
			.SkipDefault<AnalyzedStashTab>()
			.SkipDefault<LogRecord>()
			.Assert();

		// Second is not public, no items.
		var ss = this._tabs[1];
		var sr = results[1];
		sr.WithDeepEqual(new LogRecord(sr.Offset, new AnalyzedStashTab(ss.RawId, null, null, null, null, ss.Kind, Array.Empty<ItemAnalysis>())))
			.SkipDefault<AnalyzedStashTab>()
			.Assert();
	}

	private LoggedStashTab[] _tabs = null!;
	private AnalysisContext _context = null!;
	private PostgresItemLog _itemLog = null!;
}