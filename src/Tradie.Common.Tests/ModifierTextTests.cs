using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tradie.Common.Tests; 

[TestClass]
public class ModHashTests {
	[DataTestMethod]
	[DataRow("Adds 12 Damage to Attacks", 16718495535095463526UL)]
	[DataRow("+70 to Maximum Life", 13995292880504497152UL)]
	[DataRow("Immune to Physical Damage", 11864254930252050689)]
	public void TestCalculateValueIndependentHash(string input, ulong expected) {
		ulong actual = ModifierText.CalculateValueIndependentHash(input);
		Assert.AreEqual(expected, actual);
	}
	
	[DataTestMethod]
	[DataRow("Adds 12-360 Damage to Attacks", "Adds # Damage to Attacks")]
	[DataRow("Adds 12 to 360 Damage to Attacks", "Adds # Damage to Attacks")]
	[DataRow("+70 to Maximum Life", "+# to Maximum Life")]
	[DataRow("1.1% of Physical Attack Damage Leeched as Life", "#% of Physical Attack Damage Leeched as Life")]
	[DataRow("Immune to Physical Damage", "Immune to Physical Damage")]
	[DataRow("Gain +2% to Critical Strike Chance for 2 seconds after Spending a total of 800 Mana", "Gain +#% to Critical Strike Chance for # seconds after Spending a total of # Mana")]
	public void TestMakeValueIndependent(string input, string expected) {
		string actual = ModifierText.MakeValueIndependent(input);
		Assert.AreEqual(expected, actual);
	}
	
	[DataTestMethod]
	[DataRow("Adds 12-360 Damage to Attacks", 186)]
	[DataRow("Adds 12 to 360 Damage to Attacks", 186)]
	[DataRow("+70 to Maximum Life", 70)]
	[DataRow("Immune to Physical Damage", double.NaN)]
	[DataRow("-70 to Maximum Life", -70)]
	[DataRow("2.36% of Physical Attack Damage Leeched as Life", 2.36)]
	[DataRow("Gain +2% to Critical Strike Chance for 2 seconds after Spending a total of 800 Mana", 2.0)]
	[DataRow("Destroys two Armours to create a new Armour with a combination of their properties. Sometimes, the resultant item is modified unpredictably", double.NaN)]
	public void TestExtractScalar(string input, double expected) {
		double actual = ModifierText.ExtractScalar(input);
		Assert.AreEqual(expected, actual);
	}
}