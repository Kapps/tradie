using MessagePack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Tradie.Analyzer.Models;

using KeyAttribute = MessagePack.KeyAttribute;

namespace Tradie.Analyzer.Entities;

/// <summary>
/// The type of entity that an AffixRange represents.
/// </summary>
public enum AffixRangeEntityKind : byte {
	Unknown = 0,
	Modifier = 1,
	Unique = 2
}

/// <summary>
/// Represents the valid range of values present on a modifier, often within a specific criteria.
/// If the criteria contains multiple values, the range is the range of the average of the values.
/// </summary>
/// <param name="ModHash">Hash value of the modifier this range references.</param>
/// <param name="MinValue">The lowest seen value, or null if the value contains no numeric entries.</param>
/// <param name="MaxValue">The highest seen value, or null if the value contains no numeric entries.</param>
/// <param name="EntityKind">Indicates if this range applies to a unique modifier or generic modifier.</param>
/// <param name="ModCategory">Indicates whether this range applies to an explicit, implicit, or enchant modifier.</param>
[DataContract, MessagePackObject]
[Index(nameof(AffixRange.EntityKind))]
[Index(nameof(AffixRange.ModHash))]
[Index(nameof(AffixRange.ModCategory))]
public record AffixRange(
	[property:DataMember, Required, Key(0)] ulong ModHash,
	[property:DataMember, Key(1)] float? MinValue,
	[property:DataMember, Key(2)] float? MaxValue,
	[property:DataMember, Required, Key(3)] AffixRangeEntityKind EntityKind,
	[property:DataMember, Required, Key(4)] ModKindCategory ModCategory
);