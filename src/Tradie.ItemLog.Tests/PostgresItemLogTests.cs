using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Repos;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests;

[TestClass]
public class PostgresItemLogTests : TestBase {
	protected override void Initialize() {
		this._tabs = new LoggedStashTab[] {
			new("public", DateTime.UtcNow, DateTime.UtcNow,  "name1", "lcn1", "an1", "league1", "kind1", new LoggedItem[] {
				new("itemId1", new Dictionary<ushort, IAnalyzedProperties>() {
					{1, new ItemTypeAnalysis(34)}
				})
			}),
			new("id2", DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, "kind2", new LoggedItem[]{})
		};
		
		this._itemLog = new(this._context);
		
		this._context.LoggedStashTabs.AddRange(this._tabs);
		this._context.SaveChanges();
	}

	[TestMethod]
	public async Task TestGetItems() {
		var res = this._itemLog.GetItems(new ItemLogOffset(null), CancellationToken.None);
		var results = await res.ToArrayAsync();
		
		results.ShouldDeepEqual(this._tabs);
	}

	private LoggedStashTab[] _tabs = null!;
	private AnalysisContext _context = null!;
	private PostgresItemLog _itemLog;
}