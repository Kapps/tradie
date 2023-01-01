using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.ItemLogBuilder.Elastic;
using Tradie.ItemLogBuilder.Elastic.Models;
using Tradie.ItemLogBuilder.Postgres;
using Tradie.TestUtils;

namespace Tradie.ItemLog.Tests.Elastic;

[TestClass]
public class ElasticLogBuilderTests : TestBase {
	protected override void Initialize() {
		this._elasticClient = new();
		this._logBuilder = new(this._elasticClient);
	}

	[TestMethod]
	public async Task TestAppendEntries() {
		var cancellationToken = TestUtils.TestUtils.CreateCancellationToken();
		var item1 = new ItemAnalysis("first", new ItemTypeAnalysis(12));
		var item2 = new ItemAnalysis("second",
			new ItemAffixesAnalysis(new Analyzer.Models.Affix[] {
				new(123, 1, Analyzer.Models.ModKind.Explicit),
				new(234, 1.4, Analyzer.Models.ModKind.Explicit)
			}, 1, 1)
		);

		var records = new LogRecord[] {
			new(new ItemLogOffset("a"), new AnalyzedStashTab("first", "first", null, null, null, null, new[] { item1 })),
			new(new ItemLogOffset("b"), new AnalyzedStashTab("first", "first", null, null, null, null, new[] { item2 })),
		};

		await this._logBuilder.AppendEntries(records.ToAsyncEnumerable(), cancellationToken);

		var resp = await _elasticClient.SearchAsync<LoggedTab>(m=>
			m.Query(q=>
				q.QueryString(c=>
					c.Fields(d=>
						d.Field(c=>c.StashTabId)
					).Query("first")
				)
			)
		, cancellationToken);
		Console.WriteLine(resp.Documents.Single());
		//Assert.AreEqual(resp.Documents.Single(), items[0]);
	}

	private ElasticLogBuilder _logBuilder;
	private TradieElasticClient _elasticClient;
}