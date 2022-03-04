using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
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
	public async Task TestConvert_WithMixedItems() {
		var items = new[] {
			await ItemUtils.ReadTestItem("2mod"),
		};

		var inputHashes = new[] {
			ModifierText.CalculateValueIndependentHash("10% increased Movement Speed"),
			ModifierText.CalculateValueIndependentHash("+23% to Cold Resistance"),
		};

		var existingMods = new[] {
			new Modifier(inputHashes[0], "#% increased Movement Speed") {
				Id = 3,
			},
		};

		var missingMods = new[] {
			new Modifier(inputHashes[1], "+#% to Cold Resistance"),
		};
		
		this._repo.Setup(c => c.LoadByModHash(
			It.Is<ulong[]>(hashes => hashes.IsDeepEqual(inputHashes))
		)).ReturnsAsync(existingMods);

		this._repo.Setup(c => c.Insert(
			It.Is<IEnumerable<Modifier>>(modifiers => modifiers.ToArray().IsDeepEqual(missingMods))
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
		};

		var existingMods = new[] {
			new Modifier(inputHashes[0], "#% increased Movement Speed") {
				Id = 3,
			},
			new Modifier(inputHashes[1], "+#% to Cold Resistance") {
				Id = 4,
			},
		};
		
		this._repo.Setup(c => c.LoadByModHash(
			It.Is<ulong[]>(hashes => hashes.IsDeepEqual(inputHashes))
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
		};

		var existingMods = Array.Empty<Modifier>();
		var missingMods = new[] {
			new Modifier(inputHashes[0], "#% increased Movement Speed"),
			new Modifier(inputHashes[1], "+#% to Cold Resistance"),
		};
		
		this._repo.Setup(c => c.LoadByModHash(
			It.Is<ulong[]>(hashes => hashes.IsDeepEqual(inputHashes))
		)).ReturnsAsync(existingMods);
		
		this._repo.Setup(c => c.Insert(
			It.Is<IEnumerable<Modifier>>(modifiers => modifiers.ToArray().IsDeepEqual(missingMods))
		)).Returns(Task.CompletedTask);

		await this._converter.ConvertModifiers(items);
	}

	private Mock<IModifierRepository> _repo = null!;
	private AnalyzingModConverter _converter = null!;
}