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
	[DataRow("Adds 12-360 Damage to Attacks", "Adds #-# Damage to Attacks")]
	[DataRow("+70 to Maximum Life", "+# to Maximum Life")]
	[DataRow("Immune to Physical Damage", "Immune to Physical Damage")]
	public void TestCalculateValueIndependentHash(string input, string expected) {
		string actual = ModifierText.MakeValueIndependent(input);
		Assert.AreEqual(expected, actual);
	}
	
	[DataTestMethod]
	[DataRow("Adds 12-360 Damage to Attacks", 186)]
	[DataRow("+70 to Maximum Life", 70)]
	[DataRow("Immune to Physical Damage", double.NaN)]
	public void TestExtractScalar(string input, double expected) {
		double actual = ModifierText.ExtractScalar(input);
		Assert.AreEqual(expected, actual);
	}
}