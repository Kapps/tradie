using Grpc.Core;
using System.Runtime.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.ItemLog;
using Tradie.Proto.Indexer;

namespace Tradie.Indexer.Search;

public class SearchController : SearchService.SearchServiceBase {
	public SearchController(IItemLog itemLog) {
		this._itemLog = itemLog;
	}
	
	public override async Task<SearchResponse> SearchGear(SearchRequest request, ServerCallContext context) {
		throw new NotImplementedException();

		return new SearchResponse(new SearchResponse());
	}

	private readonly IItemLog _itemLog;
}
