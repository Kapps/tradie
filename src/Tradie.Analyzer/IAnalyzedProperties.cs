﻿using Tradie.Analyzer.Analyzers;

namespace Tradie.Analyzer; 

/// <summary>
/// An extended set of properties for an item that are calculated from analyzing a raw item.
/// </summary>
[MessagePack.Union(0, typeof(ItemAffixesAnalysis))]
[MessagePack.Union(1, typeof(ItemTypeAnalysis))]
[MessagePack.Union(2, typeof(TradeListingAnalysis))]
[MessagePack.Union(3, typeof(ItemDetailsAnalysis))]
public interface IAnalyzedProperties {
	/*/// <summary>
	/// Serializes these properties in binary format, in such a way that a reader can retrieve them.
	/// This may be just IDs, or may be other data required.
	/// </summary>
	void Serialize(BinaryWriter writer);*/
}