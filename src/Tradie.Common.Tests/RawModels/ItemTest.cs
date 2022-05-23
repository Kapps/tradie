using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Tradie.Common.RawModels;
using Serializer = SpanJson.JsonSerializer.Generic.Utf16;

namespace Tradie.Common.Tests.RawModels; 

[TestClass]
public class ItemTest {
	[TestMethod]
	public void TestBoots() {
		var actual = loadItem("boots");
		var expected = new Item(
			id: "8539e17c6080305f4c9996a0b47f40ad7d18780d008d21667895adbbfea09580",
			verified: false,
			width: 2, height: 2,
			iconPath: @"https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9Cb290cy9Cb290c1N0ckludDEiLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MX1d/0519faf993/BootsStrInt1.png",
			league: "Scourge",
			name: "Skull Span", typeLine: "Riveted Boots", baseType: "Riveted Boots",
			identified: true, itemLevel: 85, frameType: 2, x: 0, y: 0, inventoryId: "Stash33",
			sockets: new[] {
				new ItemSocket(0, "S", "R"),
				new ItemSocket(0, "S", "R"),
				new ItemSocket(1, "I", "B"),
			},
			properties: new[] {
				new ItemProperty("Armour", new[] { new ItemPropertyValue("94", 1) }, 16),
				new ItemProperty("Energy Shield", new[] { new ItemPropertyValue("20", 1) }, 18),
			},
			requirements: new[] {
				new ItemProperty("Level", new[] {new ItemPropertyValue("63", 0)}, 0),
				new ItemProperty("Str", new[] { new ItemPropertyValue("35", 0) }, 1),
				new ItemProperty("Int", new[] { new ItemPropertyValue("35", 0) }, 1),
			},
			enchantMods: new[] {
				"90% increased Critical Strike Chance if you haven't Crit Recently",
			},
			explicitMods: new[] {
				"+32 to Strength",
				"33% increased Armour and Energy Shield",
				"+13 to maximum Life",
				"41% increased Stun and Block Recovery"
			},
			extendedProperties: new ExtendedItemProperties("armour", new[] { "boots" }, 2, 2),
			socketedItems: Array.Empty<Item>()
		);
	}

	[TestMethod]
	public void TestAmulet() {
		var actual = loadItem("amulet");

		var expected = new Item(
			id: "e6dc58aa1b0f28a8dde845c77175b599ad4fc44f35fa04c16569fb776f66ca5e",
			verified: false,
			width: 1, height: 1,
			iconPath:
			@"https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQW11bGV0cy9UdXJxdW9pc2VBbXVsZXQiLCJ3IjoxLCJoIjoxLCJzY2FsZSI6MX1d/605b4da0e1/TurquoiseAmulet.png",
			league: "Scourge",
			name: "Rift Idol", typeLine: "Turquoise Amulet", baseType: "Turquoise Amulet",
			identified: true, itemLevel: 82, frameType: 2, x: 6, y: 0, inventoryId: "Stash18",
			influences: new Influence(false, false, false, false, true, false),
			requirements: new[] {
				new ItemProperty("Level", new[] {new ItemPropertyValue("54", 0)}, 0),
			},
			implicitMods: new[] {
				"+17 to Dexterity and Intelligence"
			},
			craftedMods: new[] {
				"+12 to Maximum Life"
			},
			explicitMods: new[] {
				"+11 to all Attributes",
				"+12% to Chaos Damage over Time Multiplier",
				"Adds 12 to 24 Fire Damage to Attacks",
				"Items and Gems have 9% reduced Attribute Requirements"
			},
			extendedProperties: new ExtendedItemProperties("accessories", new[] {"amulet"}, 1, 3)
		);

		assertEqual(expected, actual);
	}

	private void assertEqual(Item expected, Item actual) {
		/*actual.WithDeepEqual(expected)
			.SkipDefault<Item>()
			.SkipDefault<ItemProperty>()
			.SkipDefault<ExtendedItemProperties>()
			.SkipDefault<ItemPropertyValue>()
			.Assert();*/
		var expectedJson = Serializer.Serialize(expected);
		var actualJson = Serializer.Serialize(actual);
		Assert.AreEqual(expectedJson, actualJson);
	}

	private Item loadItem(string itemName) {
		string json = File.ReadAllText($"RawModels/json/{itemName}.json");
		return Serializer.Deserialize<Item>(json);
	}
}