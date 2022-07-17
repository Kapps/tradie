using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Models;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers.Conversions;

[TestClass]
public class PseudoModCalculatorTests : TestBase {
	protected override void Initialize() {
		this._calculator = new PseudoModCalculator();
	}

	[TestMethod]
	public void TestCalculateTotalMaxLife() {
		var affixes = new Affix[] {
			new(9024037547368883040UL, 20, ModKind.Total),
			new(6442787728654046253UL, 40, ModKind.Total)
		};
		
		var results = this._calculator.CalculatePseduoMods(default, affixes).ToArray();
		CollectionAssert.AreEqual(new Affix[] {
			new(PseudoMods.TotalMaxLife.ModHash, 28, ModKind.Pseudo)
		}, results);
	}

	[TestMethod]
	public void TestCalculateTotalResists() {
		var affixes = new Affix[] {
			new(14277321269326717293UL, 20, ModKind.Total), // Single Resistance 
			new(10070184050534957371, 4, ModKind.Total), // Dual non-Chaos
			new(12269184774600982397, 15, ModKind.Total), // Chaos and non-Chaos,
			new(2570102653797474430, 3, ModKind.Total)
		};
		
		var results = this._calculator.CalculatePseduoMods(default, affixes).ToArray();
		Assert.AreEqual(3, results.Length);
		
		CollectionAssert.AreEqual(new Affix[] {
			new(PseudoMods.TotalResists.ModHash, 67, ModKind.Pseudo),
			new(PseudoMods.TotalChaosRes.ModHash, 15, ModKind.Pseudo),
			new(PseudoMods.TotalEleRes.ModHash, 52, ModKind.Pseudo)
		}, results);
	}
	
	private PseudoModCalculator _calculator = null!;
}