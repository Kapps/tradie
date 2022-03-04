using MessagePack;
using System.Runtime.Serialization;

namespace Tradie.Analyzer;

/// <summary>
/// Represents a stash tab that has been fully analyzed.
/// </summary>
/// <param name="StashTabId">The PoE unique ID for this stash tab.</param>
/// <param name="Name">The name, or note, of the tab. Can include price.</param>
/// <param name="LastCharacterName">The name of the last character the owner is known to have used.</param>
/// <param name="AccountName">The account name of the owner.</param>
/// <param name="League">The league this tab is for.</param>
/// <param name="Items">The analysis of the items contained in this tab.</param>
/// <param name="Kind">The type of stash tab, such as Currency or MapStash.</param>
[MessagePackObject]
[DataContract]
public record struct AnalyzedStashTab(
	[property: DataMember, Key(0)] string StashTabId,
	[property: DataMember, Key(1)] string? Name,
	[property: DataMember, Key(2)] string? LastCharacterName,
	[property: DataMember, Key(3)] string? AccountName,
	[property: DataMember, Key(4)] string? League,
	[property: DataMember, Key(5)] string? Kind,
	[property: DataMember, Key(6)] ItemAnalysis[] Items
);
