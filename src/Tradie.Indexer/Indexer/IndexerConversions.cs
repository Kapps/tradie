using Tradie.Indexer.Search;
using ModKey = Tradie.Analyzer.Analyzers.ModKey;

namespace Tradie.Proto.Indexer;

public partial class AffixRange {
	public static implicit operator Tradie.Indexer.Storage.AffixRange(AffixRange range) {
		return new(range.MinValue, range.MaxValue, (ModKey)range.Key);
	}
}

public partial class SearchGroup {
	public static implicit operator Tradie.Indexer.Search.SearchGroup(SearchGroup group) {
		return new((GroupKind)group.GroupKind, group.Ranges.Select(c=>(Tradie.Indexer.Storage.AffixRange)c).ToArray());
	}
}

public partial class SortOrder {
	public static implicit operator Tradie.Indexer.Search.SortOrder(SortOrder order) {
		return new((SortKind)order.SortKind, order.Modifier);
	}
}

public partial class SearchQuery {
	public static implicit operator Tradie.Indexer.Search.SearchQuery(SearchQuery query) {
		return new(query.Groups.Select(c => (Tradie.Indexer.Search.SearchGroup)c).ToArray(), query.Sort);
	}
}