namespace Tradie.Analyzer.Models;

/// <summary>
/// The type of mod that an affix represents.
/// </summary>
public enum ModKind : byte {
	Unknown = 0,
	Implicit = 1,
	Explicit = 2,
	Enchant = 3,
	Scourge = 4,
	Utility = 5,
	Fractured = 6,
	Cosmetic = 7,
	Veiled = 8,
	Crafted = 9,
	Total = 10,
	Pseudo = 11
}

/// <summary>
/// Represents a grouping of multiple ModKinds together in a specific location. 
/// </summary>
public enum ModKindCategory : byte {
	/// <summary>
	/// Enchant modifiers. Includes cosmetics.
	/// </summary>
	Enchant,
	/// <summary>
	/// Implicit modifiers. Includes scourge and utility mods.
	/// </summary>
	Implicit,
	/// <summary>
	/// Explicit modifiers. Includes fractured and veiled mods.
	/// </summary>
	Explicit,
	/// <summary>
	/// Pseudo modifiers that encompass all other modifiers or combinations of them.
	/// </summary>
	Pseudo
}

public static class ModKindExtensions {
	public static ModKindCategory GetCategory(this ModKind kind) {
		switch(kind) {
			case ModKind.Enchant:
			case ModKind.Cosmetic:
				return ModKindCategory.Enchant;
			case ModKind.Implicit:
			case ModKind.Scourge:
			case ModKind.Utility:
				return ModKindCategory.Implicit;
			case ModKind.Explicit:
			case ModKind.Fractured:
			case ModKind.Veiled:
			case ModKind.Crafted:
			case ModKind.Unknown:
				return ModKindCategory.Explicit;
			case ModKind.Pseudo:
				return ModKindCategory.Pseudo;
			default:
				throw new ArgumentOutOfRangeException(nameof(kind));
		}
	}
}