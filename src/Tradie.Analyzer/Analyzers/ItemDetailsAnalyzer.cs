using MessagePack;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers;

/// <summary>
/// An analyzed that records basic properties, such as ilvl and influence.
/// </summary>
public class ItemDetailsAnalyzer : IItemAnalyzer {
	
	public static ushort Id { get; } = KnownAnalyzers.ItemDetails;
	
	public ValueTask AnalyzeItems(AnalyzedItem[] items) {
		foreach(var item in items) {
			var raw = item.RawItem;
			var flags = GetFlags(raw);
			var influences = GetInfluences(raw);
			var rarity = (ItemRarity)raw.FrameType.GetValueOrDefault();
			var requirements = RequirementParser.MapRequirements(raw, true);
			var analyzed = new ItemDetailsAnalysis(raw.Name, flags, influences, (byte?)raw.ItemLevel, rarity, requirements);
			item.Analysis.PushAnalysis(Id, analyzed);
		}

		return ValueTask.CompletedTask;
	}
	
	public ValueTask DisposeAsync() {
		return ValueTask.CompletedTask;
	}

	private ItemFlags GetFlags(Item raw) {
		ItemFlags flags = 0;
		
		foreach(var flagPair in new[] {
			        (ItemFlags.Corrupted, raw.Corrupted),
			        (ItemFlags.Fractured, raw.Fractured),
			        (ItemFlags.Mirrored, raw.Mirrored),
			        (ItemFlags.Relic, raw.Relic),
			        (ItemFlags.Replica, raw.Replica),
			        (ItemFlags.Synthesized, raw.Synthesized),
			        (ItemFlags.Veiled, raw.Veiled)
		        }) {
			if(flagPair.Item2)
				flags |= flagPair.Item1;
		}

		return flags;
	}

	private InfluenceKind GetInfluences(Item raw) {
		if(!raw.Influences.HasValue) {
			return default;
		}

		var rawInf = raw.Influences.Value;
		InfluenceKind influences = 0;
		foreach(var flagPair in new[] {
			        (InfluenceKind.Crusader, rawInf.Crusader),
			        (InfluenceKind.Elder, rawInf.Elder),
			        (InfluenceKind.Hunter, rawInf.Hunter),
			        (InfluenceKind.Redeemer, rawInf.Redeemer),
			        (InfluenceKind.Shaper, rawInf.Shaper),
			        (InfluenceKind.Warlord, rawInf.Warlord)
		        }) {

			if(flagPair.Item2) {
				influences |= flagPair.Item1;
			}
		}

		return influences;
	}
}

/// <summary>
/// Analyzes an item for its basic properties.
/// </summary>
/// <param name=">Name">The name of the item.</param>
/// <param name="Flags">Any additional flags, such as being corrupted.</param>
/// <param name="Influences">The influences applied to the item, if any.</param>
/// <param name="ItemLevel">The item level, if applicable.</param>
[DataContract, MessagePackObject]
public readonly record struct ItemDetailsAnalysis(
	[property:DataMember, Key(0)] string Name,
	[property:DataMember, Key(1), JsonConverter(typeof(FlagsEnumJsonConverter<ItemFlags>))] ItemFlags Flags,
	[property:DataMember, Key(2), JsonConverter(typeof(FlagsEnumJsonConverter<InfluenceKind>))] InfluenceKind Influences,
	[property:DataMember, Key(3)] byte? ItemLevel,
	[property:DataMember, Key(4), JsonConverter(typeof(FlagsEnumJsonConverter<ItemRarity>))] ItemRarity Rarity,
	[property:DataMember, Key(5)] Requirements? Requirements
) : IAnalyzedProperties;
	
/// <summary>
/// Additional flags that apply to an item, such as being corrupted.
/// </summary>
[Flags]
public enum ItemFlags : ushort {
	None = 0,
	Corrupted = 1,
	Mirrored = 2,
	Veiled = 4,
	Relic = 8,
	Replica = 16,
	Synthesized = 32,
	Fractured = 64
}

/// <summary>
/// The types of influences present on an item.
/// </summary>
[Flags]
public enum InfluenceKind : ushort {
	None = 0,
	Redeemer = 1,
	Crusader = 2,
	Warlord = 4,
	Hunter = 8,
	Shaper = 16,
	Elder = 32
}

/// <summary>
/// The rarity, or frame type, of an item.
/// </summary>
public enum ItemRarity : byte {
	Normal = 0,
	Magic = 1,
	Rare = 2,
	Unique = 3,
	Gem = 4
}