using MessagePack;
using System.Runtime.Serialization;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer; 

/// <summary>
/// Provides a way of storing analyzed properties about a raw item.
/// </summary>
[DataContract, MessagePackObject]
public record struct AnalyzedItem(Item RawItem) {
	/// <summary>
	/// The raw item being analyzed.
	/// </summary>
	[IgnoreMember, IgnoreDataMember]
	public readonly Item RawItem = RawItem;
	/// <summary>
	/// The analysis done on this item.
	/// </summary>
	[DataMember, Key(1)]
	public ItemAnalysis Analysis = new(RawItem.Id);
	/// <summary>
	/// Unique ID of the item; consistent when being traded.
	/// </summary>
	[DataMember, Key(0)]
	public string Id => this.RawItem.Id;
}