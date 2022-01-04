using MessagePack;
using System.Runtime.Serialization;

namespace Tradie.Analyzer;

/// <summary>
/// Represents a stash tab that has been fully analyzed.
/// </summary>
[MessagePackObject]
[DataContract]
public record struct AnalyzedStashTab(
	[property:DataMember, Key(0)] string StashTabId,
	[property:DataMember, Key(1)]ItemAnalysis[] Items
);
