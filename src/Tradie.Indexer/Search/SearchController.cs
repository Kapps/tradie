using Grpc.Core;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.Common.Metrics;
using Tradie.Indexer.Proto;
using Tradie.Indexer.Storage;
using Tradie.ItemLog;

namespace Tradie.Indexer.Search;

public class SearchController : SearchService.SearchServiceBase {
	public SearchController(ItemTree itemTree, IBlockSearcher blockSearcher, IMetricPublisher metricPublisher) {
		this._itemTree = itemTree;
		this._blockSearcher = blockSearcher;
		this._metricPublisher = metricPublisher;
	}
	
	public override async Task<SearchResponse> SearchGear(SearchRequest request, ServerCallContext context) {
		var sw = Stopwatch.StartNew();
		//Console.WriteLine("---------Raw---------");
		//Console.WriteLine(JsonSerializer.Serialize(request.Query, new JsonSerializerOptions() { WriteIndented = true }));
		var results = this._blockSearcher.Search(this._itemTree.Root, request.Query, 100);
		/*Console.WriteLine("--------Input--------");
		Console.WriteLine(JsonSerializer.Serialize((Tradie.Indexer.Search.SearchQuery)request.Query, new JsonSerializerOptions() { WriteIndented = true }));
		Console.WriteLine("--------Output--------");
		Console.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions() { WriteIndented = true }));
		Console.WriteLine("----------------------");*/
		Console.WriteLine($"Elapsed: {sw.Elapsed.TotalMilliseconds}");
		
		await this._metricPublisher.PublishMetric(IndexerMetrics.SearchTime, new CustomMetricDimension[] {
			new("League", "Standard"), // TODO: Get the league...
		}, sw.Elapsed.TotalMilliseconds, context.CancellationToken);

		var response = new SearchResponse() {
			Ids = {
				results.Select(c => c.Id)
			}
		};
		return response;
	}

	private readonly ItemTree _itemTree;
	private readonly IBlockSearcher _blockSearcher;
	private readonly IMetricPublisher _metricPublisher;
}
