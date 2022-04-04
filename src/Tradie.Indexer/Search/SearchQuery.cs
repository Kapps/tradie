using System.Runtime.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Indexer.Storage;

namespace Tradie.Indexer.Search;

/// <summary>
/// Represents a search query that contains one or more groups to match against a block or item.
/// </summary>
[DataContract]
public record struct SearchQuery(
	[property:DataMember(Order = 1)]
	SearchGroup[] Groups,
	[property:DataMember(Order = 2)]
	SortOrder Sort
);

[DataContract]
public record struct SearchGroup(
	[property:DataMember(Order = 1)]
	GroupKind Kind,
	[property:DataMember(Order = 2)]
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

[DataContract]
public record struct SortOrder(
	[property:DataMember(Order = 1)]
	SortKind Kind,
	[property:DataMember(Order = 2, IsRequired = false)]
	ModKey? Mod
);