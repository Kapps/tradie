namespace Tradie.Indexer.Search;

/// <summary>
/// Represents a search query that contains one or more groups to match against a block or item.
/// </summary>
/// <param name="Groups"></param>
/// <param name="Sort"></param>
public record struct SearchQuery(
	SearchGroup[] Groups,
	SortOrder Sort
);

public record struct SearchGroup(
	GroupKind Kind,
	AffixRange[] Ranges
);

public enum GroupKind {
	And = 1,
	Sum = 2,
}

/// <summary>
/// Indicates how to sort the items returned from a SearchQuery.
/// </summary>
public enum SortKind {
	Price = 1,
	Modifier = 2
}

public record struct SortOrder(
	SortKind Kind,
	ModKey? Mod
);