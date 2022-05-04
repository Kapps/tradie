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
using Tradie.Common;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests;

[TestClass]
public class PostgresItemLogTests : TestBase {
	protected override void Initialize() {
		this._tabs = new LoggedStashTab[] {
			new("public-other-league", DateTime.UtcNow, DateTime.UtcNow,  "name1", "lcn1", "an1", "league1", "kind1", MessagePackSerializer.Serialize(new ItemAnalysis[] {
				new("itemId1", new Dictionary<ushort, IAnalyzedProperties>() {
					{1, new ItemTypeAnalysis(34)}
				})
			}, MessagePackedStashTabSerializer.SerializationOptions)),
			new("public-in-league", DateTime.UtcNow, DateTime.UtcNow,  "name2", "lcn2", "an2", TradieConfig.League, "kind2", MessagePackSerializer.Serialize(new ItemAnalysis[] {
				new("itemId2", new Dictionary<ushort, IAnalyzedProperties>() {
					{1, new ItemTypeAnalysis(34)}
				})
			}, MessagePackedStashTabSerializer.SerializationOptions)),
			new("id2", DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, "kind2", null)
		};
		
		this._itemLog = new(this._context);
		
		this._context.LoggedStashTabs.AddRange(this._tabs);
		this._context.SaveChanges();
	}

	protected override void Cleanup() {
		TradieConfig.League = "Anarchy";
	}

	[TestMethod]
	public async Task TestGetItems_NoLeague() {
		TradieConfig.League = null;
		var res = this._itemLog.GetItems(new ItemLogOffset(null), CancellationToken.None);
		var results = await res.ToArrayAsync();
		
		Assert.AreEqual(3, results.Length);
		// First tab is public with items.
		var tab0 = this._tabs[0];
		var res0 = results[0];
		var tab0Items =
			MessagePackSerializer.Deserialize<ItemAnalysis[]>(tab0.PackedItems, MessagePackedStashTabSerializer.SerializationOptions);
		res0.WithDeepEqual(new LogRecord(res0.Offset, new AnalyzedStashTab(tab0.RawId, tab0.Name, tab0.LastCharacterName, tab0.Owner,
			tab0.League, tab0.Kind, tab0Items)))
			.SkipDefault<AnalyzedStashTab>()
			.SkipDefault<LogRecord>()
			.Assert();
		
		// Second tab is public with items.
		var tab1 = this._tabs[1];
		var res1 = results[1];
		var tab1Items =
			MessagePackSerializer.Deserialize<ItemAnalysis[]>(tab1.PackedItems, MessagePackedStashTabSerializer.SerializationOptions);
		res1.WithDeepEqual(new LogRecord(res1.Offset, new AnalyzedStashTab(tab1.RawId, tab1.Name, tab1.LastCharacterName, tab1.Owner,
				"Anarchy", tab1.Kind, tab1Items)))
			.SkipDefault<AnalyzedStashTab>()
			.SkipDefault<LogRecord>()
			.Assert();

		// Third is not public, no items.
		var tab2 = this._tabs[2];
		var res2 = results[2];
		res2.WithDeepEqual(new LogRecord(res2.Offset, new AnalyzedStashTab(tab2.RawId, null, null, null, null, tab2.Kind, Array.Empty<ItemAnalysis>())))
			.SkipDefault<AnalyzedStashTab>()
			.Assert();
	}
	
	[TestMethod]
	public async Task TestGetItems_WithLeague() {
		var res = this._itemLog.GetItems(new ItemLogOffset(null), CancellationToken.None);
		var results = await res.ToArrayAsync();
		
		Assert.AreEqual(2, results.Length);
		// First tab is missing due to being in another league.
		
		// Second tab is public with items.
		var tab1 = this._tabs[1];
		var res1 = results[0];
		var tab1Items =
			MessagePackSerializer.Deserialize<ItemAnalysis[]>(tab1.PackedItems, MessagePackedStashTabSerializer.SerializationOptions);
		
		res1.WithDeepEqual(new LogRecord(res1.Offset, new AnalyzedStashTab(tab1.RawId, tab1.Name, tab1.LastCharacterName, tab1.Owner,
				"Anarchy", tab1.Kind, tab1Items)))
			.SkipDefault<AnalyzedStashTab>()
			.SkipDefault<LogRecord>()
			.Assert();

		// Third is not public, no items.
		var tab2 = this._tabs[2];
		var res2 = results[1];
		res2.WithDeepEqual(new LogRecord(res2.Offset, new AnalyzedStashTab(tab2.RawId, null, null, null, null, tab2.Kind, Array.Empty<ItemAnalysis>())))
			.SkipDefault<AnalyzedStashTab>()
			.Assert();
	}

	private LoggedStashTab[] _tabs = null!;
	private AnalysisContext _context = null!;
	private PostgresItemLog _itemLog = null!;
}