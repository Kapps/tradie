using MessagePack;
using System.Runtime.Serialization;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer; 

/// <summary>
/// Provides a way of storing analyzed properties about a raw item.
/// </summary>
[DataContract, MessagePackObject]
public readonly record struct AnalyzedItem {
	/// <summary>
	/// The raw item being analyzed.
	/// </summary>
	[IgnoreMember, IgnoreDataMember]
	public readonly Item RawItem;
	/// <summary>
	/// The analysis done on this item.
	/// </summary>
	[DataMember, Key(1)]
	public readonly ItemAnalysis Analysis;
	/// <summary>
	/// Unique ID of the item; consistent when being traded.
	/// </summary>
	[DataMember, Key(0)]
	public string Id => this.RawItem.Id;

	/// <summary>
	/// Creates a new AnalyzedItem from the given raw item.
	/// </summary>
	public AnalyzedItem(Item rawItem) {
		this.RawItem = rawItem;
		this.Analysis = new(rawItem.Id);
	}
}