using Grpc.Core;
using System.Runtime.Serialization;
using System.Text.Json;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.Indexer.Proto;
using Tradie.Indexer.Storage;
using Tradie.ItemLog;

namespace Tradie.Indexer.Search;

public class SearchController : SearchService.SearchServiceBase {
	public SearchController(ItemTree itemTree, IBlockSearcher blockSearcher) {
		this._itemTree = itemTree;
		this._blockSearcher = blockSearcher;
	}
	
	public override async Task<SearchResponse> SearchGear(SearchRequest request, ServerCallContext context) {
		var results = this._blockSearcher.Search(this._itemTree.Root, request.Query, 100);
		/*Console.WriteLine("--------Input--------");
		Console.WriteLine(JsonSerializer.Serialize((Tradie.Indexer.Search.SearchQuery)request.Query, new JsonSerializerOptions() { WriteIndented = true }));
		Console.WriteLine("--------Output--------");
		Console.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions() { WriteIndented = true }));
		Console.WriteLine("----------------------");*/

		var response = new SearchResponse() {
			Ids = {
				results.Select(c => c.Id)
			}
		};
		return response;
	}

	private readonly ItemTree _itemTree;
	private readonly IBlockSearcher _blockSearcher;
}
