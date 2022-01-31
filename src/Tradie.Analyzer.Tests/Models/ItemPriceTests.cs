using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Tests.Models {
	[TestClass]
	public class ItemPriceTests {

		[TestMethod]
		[DataTestMethod]
		[DataRow("~b/o 2 chaos", BuyoutCurrency.Chaos, 2, BuyoutKind.Offer)]
		[DataRow("~price 2 chaos", BuyoutCurrency.Chaos, 2, BuyoutKind.Fixed)]
		[DataRow("~price 4 exa", BuyoutCurrency.Exalted, 4, BuyoutKind.Fixed)]
		[DataRow("b/o 2 chaos", default, default, default)]
		[DataRow(null, default, default, default)]
		public void TestParsing(string note, BuyoutCurrency expectedCurrency, float expectedAmount, BuyoutKind expectedKind) {
			bool success = ItemPrice.TryParse(note, out var parsed);
			if(expectedCurrency == BuyoutCurrency.None) {
				Assert.IsFalse(success);
				return;
			}
			Assert.IsTrue(success);
			
			Assert.AreEqual(expectedCurrency, parsed.Currency);
			Assert.AreEqual(expectedAmount, parsed.Amount);
			Assert.AreEqual(expectedKind, parsed.Kind);
		}
	}
}