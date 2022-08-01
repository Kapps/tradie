using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Tests.Analyzers.Conversions; 

[TestClass]
public class ModConverterTests {
	[TestInitialize]
	public void Initialize() {
		this._repo = new Mock<IModifierRepository>(MockBehavior.Strict);
		this._converter = new AnalyzingModConverter(this._repo.Object);
	}

	[TestCleanup]
	public void Cleanup() {
		this._repo.VerifyAll();
	}
	
	[TestMethod]
	public async Task TestConvert_NoItems() {
		var items = Enumerable.Empty<Item>();
		var mods = await this._converter.ConvertModifiers(items);
		Assert.IsTrue(!mods.Any());
	}

	[TestMethod]
	public async Task TestConvert_Currency() {
		var items = new[] {
			await ItemUtils.ReadTestItem("exaltshard")
		};
		
		var mods = await this._converter.ConvertModifiers(items);
		Assert.IsFalse(mods.Any());

		var affixes = this._converter.ExtractAffixes(items[0]);
		Assert.IsFalse(affixes.Any());
	}

	[TestMethod]
	public async Task TestExtractAffixes_Basic() {
		var item = await ItemUtils.ReadTestItem("2mod");

		var affixes = this._converter.ExtractAffixes(item).ToArray();
		Assert.AreEqual(8, affixes.Length);
		
		affixes.ShouldDeepEqual(new Affix[] {
			new(ModifierText.CalculateValueIndependentHash("this should be ignored"), 0, ModKind.Cosmetic),
			new(ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"), 10, ModKind.Enchant),
			new(ModifierText.CalculateValueIndependentHash("20% increased Movement Speed"), 20, ModKind.Explicit),
			new(ModifierText.CalculateValueIndependentHash("+46% to Cold Resistance"), 46, ModKind.Explicit),
			new(ModifierText.CalculateValueIndependentHash("+23% to Cold Resistance"), 23, ModKind.Crafted),
			new(ModifierText.CalculateValueIndependentHash("20% increased Movement Speed"), 20, ModKind.Scourge),
			new(ModifierText.CalculateValueIndependentHash("Quality"), 20, ModKind.Property),
			new(ModifierText.CalculateValueIndependentHash("Armour"), 132, ModKind.Property),
		});
	}

	[TestMethod]
	public async Task TestConvert_WithMixedItems() {
		var items = new[] {
			await ItemUtils.ReadTestItem("2mod"),
		};

		var inputHashes = new[] {
			ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"),
			ModifierText.CalculateValueIndependentHash("+23% to Cold Resistance"),
			ModifierText.CalculateValueIndependentHash("Quality"),
			ModifierText.CalculateValueIndependentHash("Armour"),
		};

		var existingMods = new[] {
			new Modifier(inputHashes[0], "#% increased Movement Speed") {
				Id = 3,
			},
			new Modifier(inputHashes[2], "Quality") {
				Id = 4,
			},
		};

		var missingMods = new[] {
			new Modifier(inputHashes[1], "+#% to Cold Resistance"),
			new Modifier(inputHashes[3], "Armour"),
		};
		
		this._repo.Setup(c => c.LoadByModHash(
			It.Is<ulong[]>(hashes => hashes.IsDeepEqual(inputHashes)), CancellationToken.None
		)).ReturnsAsync(existingMods);

		this._repo.Setup(c => c.Insert(
			It.Is<IEnumerable<Modifier>>(modifiers => modifiers.ToArray().IsDeepEqual(missingMods)), CancellationToken.None
		)).Returns(Task.CompletedTask);

		await this._converter.ConvertModifiers(items);
	}
	
	[TestMethod]
	public async Task TestConvert_WithExistingItems() {
		var items = new[] {
			await ItemUtils.ReadTestItem("2mod"),
		};

		var inputHashes = new[] {
			ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"),
			ModifierText.CalculateValueIndependentHash("+23% to Cold Resistance"),
			ModifierText.CalculateValueIndependentHash("Quality"),
			ModifierText.CalculateValueIndependentHash("Armour"),
		};

		var existingMods = new[] {
			new Modifier(inputHashes[0], "#% increased Movement Speed") {
				Id = 3,
			},
			new Modifier(inputHashes[1], "+#% to Cold Resistance") {
				Id = 4,
			},
			new Modifier(inputHashes[2], "Quality") {
				Id = 5,
			},
			new Modifier(inputHashes[3], "Armour") {
				Id = 6,
			},
		};
		
		this._repo.Setup(c => c.LoadByModHash(
			It.Is<ulong[]>(hashes => hashes.IsDeepEqual(inputHashes)), CancellationToken.None
		)).ReturnsAsync(existingMods);

		await this._converter.ConvertModifiers(items);
	}

	[TestMethod]
	public async Task TestConvert_WithAllMissingItems() {
		var items = new[] {
			await ItemUtils.ReadTestItem("2mod"),
		};

		var inputHashes = new[] {
			ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"),
			ModifierText.CalculateValueIndependentHash("+23% to Cold Resistance"),
			ModifierText.CalculateValueIndependentHash("Quality"),
			ModifierText.CalculateValueIndependentHash("Armour"),
		};

		var existingMods = Array.Empty<Modifier>();
		var missingMods = new[] {
			new Modifier(inputHashes[0], "#% increased Movement Speed"),
			new Modifier(inputHashes[1], "+#% to Cold Resistance"),
			new Modifier(inputHashes[2], "Quality"),
			new Modifier(inputHashes[3], "Armour"),
		};
		
		this._repo.Setup(c => c.LoadByModHash(
			It.Is<ulong[]>(hashes => hashes.IsDeepEqual(inputHashes)), CancellationToken.None
		)).ReturnsAsync(existingMods);
		
		this._repo.Setup(c => c.Insert(
			It.Is<IEnumerable<Modifier>>(modifiers => modifiers.ToArray().IsDeepEqual(missingMods)), CancellationToken.None
		)).Returns(Task.CompletedTask);

		await this._converter.ConvertModifiers(items);
	}

	private Mock<IModifierRepository> _repo = null!;
	private AnalyzingModConverter _converter = null!;
}