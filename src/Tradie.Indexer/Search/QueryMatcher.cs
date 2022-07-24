using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;
using Tradie.Indexer.Storage;

namespace Tradie.Indexer.Search;

// Intentionally static and non-interface to avoid virtual calls.

internal static class QueryMatcher {
	public static bool IsMatch(Item item, SearchQuery query) {
		foreach(var group in query.Groups) {
			switch(group.Kind) {
				case GroupKind.And:
					foreach(var searchRange in group.Ranges) {
						float sum = item.GetAffixValue(searchRange.Modifier);
						if(sum == 0) {
							return false;
						}

						if(sum < searchRange.MinValue || sum > searchRange.MaxValue)
							return false;
					}
					break;
				case GroupKind.Sum:
					// For sums, we'll consider it a match if we have any mod being searched for.
					bool any = false;
					foreach(var searchRange in group.Ranges) {
						var range = item.FindAffix(searchRange.Modifier);
						if(range.HasValue) {
							any = true;
							break;
						}
					}

					if(!any)
						return false;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(group.Kind));
			}
		}

		return true;
	}
	
	public static bool IsMatch(ItemTreeNode treeNode, SearchQuery query) {
		foreach(var group in query.Groups) {
			switch(group.Kind) {
				case GroupKind.And:
					foreach(var searchRange in group.Ranges) {
						var affixRange = treeNode.Affixes.Get(searchRange.Modifier.ModHash);
						if(affixRange.ModHash == 0)
							return false;
						
						ref var valueRange = ref affixRange.GetRangeForModKind(searchRange.Modifier.Kind);
						if(searchRange.MinValue > valueRange.MaxValue || searchRange.MaxValue < valueRange.MinValue)
							return false;
					}
					break;
				case GroupKind.Sum:
					// For sums, we'll consider it a match if we have any mod being searched for.
					// This should actually be a match if it contains a value greater than the lowest value in the set.
					bool any = false;
					foreach(var searchRange in group.Ranges) {
						var range = treeNode.Affixes.Get(searchRange.Modifier.ModHash);
						if(range.ModHash != 0) {
							any = true;
							break;
						}
					}

					if(!any)
						return false;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(group.Kind));
			}
		}

		return true;
	}
}