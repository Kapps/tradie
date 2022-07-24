using Tradie.Analyzer.Models;
using Tradie.Analyzer.Proto;
using Tradie.Indexer.Search;

namespace Tradie.Indexer.Proto;

public partial class SearchRange {
	public static implicit operator Tradie.Indexer.Search.SearchRange(SearchRange range) {
		return new(range.Key, range.MinValue, range.MaxValue);
	}
}

public partial class SearchGroup {
	public static implicit operator Tradie.Indexer.Search.SearchGroup(SearchGroup group) {
		return new((GroupKind)group.GroupKind, group.Ranges.Select(c=>(Tradie.Indexer.Search.SearchRange)c).ToArray());
	}
}

public partial class SortOrder {
	public static implicit operator Tradie.Indexer.Search.SortOrder (SortOrder order) {
		return new((SortKind)order.SortKind, order.Modifier);
	}
}

public partial class SearchQuery {
	public static implicit operator Tradie.Indexer.Search.SearchQuery(SearchQuery query) {
		return new(query.Groups.Select(c => (Tradie.Indexer.Search.SearchGroup)c).ToArray(), query.Sort);
	}
}