using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers.Conversions;

/// <summary>
/// Allows conversion from raw modifiers on items to analyzed repository modifiers.
/// </summary>
public interface IModConverter : IAsyncDisposable {
	/// <summary>
	/// Converts all raw modifiers found on each of the given items
	/// to the appropriate entity modifier entry.
	/// </summary>
	ValueTask<IEnumerable<Modifier>> ConvertModifiers(IEnumerable<Item> items);

	/// <summary>
	/// Extracts the ModValues from each item and the original converted modifiers.
	/// </summary>
	IEnumerable<Affix> ExtractAffixes(Item item);
}

/// <summary>
/// An IModConverter that analyzes all modifiers and inserts missing ones into a repository.
/// </summary>
public class AnalyzingModConverter : IModConverter {
	public AnalyzingModConverter(IModifierRepository repo) {
		this._repo = repo;
	}
	
	public IEnumerable<Affix> ExtractAffixes(Item item) {
		var affixes = MapValues(item.CosmeticMods, ModKind.Cosmetic).ConcatMany(
			MapValues(item.EnchantMods, ModKind.Enchant),
			MapValues(item.ExplicitMods, ModKind.Explicit),
			MapValues(item.CraftedMods, ModKind.Crafted),
			MapValues(item.FracturedMods, ModKind.Fractured),
			MapValues(item.ImplicitMods, ModKind.Implicit),
			MapValues(item.ScourgeMods, ModKind.Scourge),
			MapValues(item.UtilityMods, ModKind.Utility),
			MapValues(item.VeiledMods, ModKind.Veiled)
		);

		return affixes;
	}
	
	public ValueTask DisposeAsync() {
		return this._repo.DisposeAsync();
	}

	private static IEnumerable<Affix>? MapValues(string[]? itemAffixes, ModKind kind) {
		if(itemAffixes == null || itemAffixes.Length == 0 || itemAffixes.All(String.IsNullOrWhiteSpace)) {
			return null;
		}

		return itemAffixes
			.Where(c=>!String.IsNullOrWhiteSpace(c))
			.Select(c => {
				var hash = ModifierText.CalculateValueIndependentHash(c);
				var scalar = ModifierText.ExtractScalar(c);
				return new Affix(hash, scalar, kind);
			})
			.ToArray();
	}

	public async ValueTask<IEnumerable<Modifier>> ConvertModifiers(IEnumerable<Item> items) {
		var modTexts = items.SelectMany(c => c.EnchantMods.ConcatMany(
			c.ExplicitMods, c.FracturedMods, c.ImplicitMods,
			c.ScourgeMods, c.UtilityMods, c.VeiledMods
		)).Where(c=>!String.IsNullOrWhiteSpace(c));

		var modifiers = modTexts.Select(c => new Modifier(
			ModifierText.CalculateValueIndependentHash(c),
			ModifierText.MakeValueIndependent(c)
		)).DistinctBy(c=>c.ModHash).ToArray();

		if(!modifiers.Any()) {
			return Enumerable.Empty<Modifier>();
		}

		var hashes = modifiers.Select(c => c.ModHash).ToArray();
		var existing = (await this._repo.LoadByModHash(hashes)).ToDictionary(c => c.ModHash);
		var missing = modifiers.Where(c => !existing.ContainsKey(c.ModHash)).ToArray();

		if(missing.Any()) {
			await this._repo.Insert(missing);
		}

		var allMods = existing.Values.Concat(missing).ToArray();
		return allMods;
	}

	private readonly IModifierRepository _repo;
}