using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests;

[TestClass]
public class AnalyzedPropertyCollectionJsonConverterTests : TestBase {
	[TestMethod]
	public void TestDeserialization_FullObject() {
		string json =
			@"{""1"": {""ItemTypeId"": 159}, ""2"": {""Affixes"": []}, ""3"": {""X"": 0, ""Y"": 2, ""Price"": {""Kind"": ""None"", ""Amount"": 0, ""Currency"": ""None""}}, ""4"": {""Name"": """", ""Flags"": 65, ""ItemLevel"": 86, ""Influences"": """"} }";
		var props = JsonSerializer.Deserialize<AnalyzedPropertyCollection>(json, new JsonSerializerOptions() {
			Converters = {
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)
			}
		});
		Assert.AreEqual(4, props.Count);
		Assert.AreEqual(159, props.Get<ItemTypeAnalysis>(KnownAnalyzers.ItemType).ItemTypeId);
		Assert.AreEqual(ItemFlags.Corrupted | ItemFlags.Fractured, props.Get<ItemDetailsAnalysis>(KnownAnalyzers.ItemDetails).Flags);
		Assert.AreEqual(new TradeListingAnalysis(0, 2, new ItemPrice(Currency.None, 0, BuyoutKind.None), null), props.Get<TradeListingAnalysis>(KnownAnalyzers.TradeAttributes));
		Assert.AreEqual(0, props.Get<ItemAffixesAnalysis>(KnownAnalyzers.Modifiers).Affixes.Length);
	}

	[TestMethod]
	public void TestSerialization_FullObject() {
		var obj = new AnalyzedPropertyCollection();
		obj.Add(KnownAnalyzers.ItemDetails,
			new ItemDetailsAnalysis("foo", ItemFlags.Corrupted | ItemFlags.Fractured, InfluenceKind.None, 86));

		string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions() {
			Converters = {
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)
			}
		});

		string expected =
			@"{""4"":{""Name"":""foo"",""Flags"":65,""Influences"":0,""ItemLevel"":86}}";
		Assert.AreEqual(expected, json);
	}
}