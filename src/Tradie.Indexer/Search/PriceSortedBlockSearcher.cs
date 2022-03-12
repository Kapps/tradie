namespace Tradie.Indexer.Search;

/// <summary>
/// A service to search a recursive affix block for items matching a query. 
/// </summary>
public interface IBlockSearcher {
	Item[] Search(AffixBlock block, SearchQuery query, int count);
}

/// <summary>
/// A block searcher optimized for searching through blocks that are ordered by the price of items.
/// </summary>
public class PriceSortedBlockSearcher : IBlockSearcher {
	public Item[] Search(AffixBlock block, SearchQuery query, int count) {
		if(query.Sort.Kind != SortKind.Price)
			throw new NotImplementedException();

		var results = new SortedSet<SearchResult>();
		SearchBlock(block, query, results, count);

		return results.Select(c => c.Item).ToArray();
	}

	private void SearchBlock(AffixBlock block, SearchQuery query, SortedSet<SearchResult> results, int count) {
		// All blocks are sorted in order of the price of the items in them.
		// So for the default sorting of chaos equiv, start iteration at index 0 always and do depth-first.
		// Because we iterate in order of price, if we ever reach the desired count matching items, we know it's the count cheapest items.
		if(results.Count >= count && query.Sort.Kind == SortKind.Price) {
			return;
		}

		if(!QueryMatcher.IsMatch(block, query)) {
			return;
		}

		if(block.Kind == BlockKind.Node) {
			foreach(var child in block.Blocks!) {
				SearchBlock(child, query, results, count);
			}
		} else if(block.Kind == BlockKind.Leaf) {
			foreach(var item in block.Items!) {
				if(QueryMatcher.IsMatch(item, query)) {
					var sort = this.GetSortPriority(item, query.Sort);
					results.Add(new SearchResult(item, sort));
					if(results.Count >= count && query.Sort.Kind == SortKind.Price) {
						return;
					}
				}
			}
		} else {
			throw new ArgumentOutOfRangeException();
		}
	}

	private int GetSortPriority(Item item, SortOrder order) {
		switch(order.Kind) {
			case SortKind.Price:
				return (int)(item.ChaosEquivalentPrice * 1000);
			case SortKind.Modifier:
				var affix = item.FindAffix(order.Mod!.Value);
				if(affix == null) {
					return -1;
				}

				return (int)(affix.Value.Value * 1000); // Account for decimal values being truncated.
			default:
				throw new ArgumentOutOfRangeException(nameof(order));
		}
	}
}

internal readonly record struct SearchResult(Item Item, int SortOrder) : IComparable<SearchResult> {
	public int CompareTo(SearchResult other) {
		return SortOrder.CompareTo(other.SortOrder);
	}
}
