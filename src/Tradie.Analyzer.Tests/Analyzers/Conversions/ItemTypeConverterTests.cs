using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.Analyzer.Entities;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers.Conversions;

[TestClass]
public class ItemTypeConverterTests : TestBase {
	protected override void Initialize() {
		this._converter = new(TestUtils.TestUtils.CreateLogger<ItemTypeConverter>());
	}

	[TestMethod]
	public async Task TestItemTypeConversion() {
		var raw = await ItemUtils.ReadTestItem("boots");
		var converted = this._converter.ConvertFromRaw(raw);
		converted.ShouldDeepEqual(new ItemType() {
			Category = "armour", Name = "Hydrascale Boots",
			Height = 2, Width = 2, Subcategories = new[] { "boots" },
			IconUrl = "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9Cb290cy9Cb290c1N0ckRleDIiLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MX1d/d5b5430d4c/BootsStrDex2.png",
			Requirements = new Requirements(56, 56, 0, 60)
		});
	}

	[TestMethod]
	public async Task TestRequiresUpdate() {
		var existing = this._converter.ConvertFromRaw(await ItemUtils.ReadTestItem("boots"));
		var incoming = this._converter.ConvertFromRaw(await ItemUtils.ReadTestItem("boots"));
		
		Assert.IsFalse(this._converter.RequiresUpdate(existing, incoming));

		existing.Subcategories = null;
		Assert.IsTrue(this._converter.RequiresUpdate(existing, incoming));
	}

	[TestMethod]
	public void TestMergeFrom() {
		var existing = new ItemType() {
			Id = 6,
			IconUrl = "foo",
			Requirements = null
		};

		var incoming = new ItemType() {
			IconUrl = "bar",
			Requirements = new(12, 12, 12, 12)
		};
		
		this._converter.MergeFrom(existing, incoming);
		
		Assert.AreEqual("bar", existing.IconUrl);
		Assert.AreEqual(6, existing.Id);
		Assert.AreEqual(new Requirements(12, 12, 12, 12), existing.Requirements);
	}

	private ItemTypeConverter _converter = null!;
}