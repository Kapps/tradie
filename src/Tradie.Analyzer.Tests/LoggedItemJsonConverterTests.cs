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
public class LoggedItemJsonConverterTests : TestBase {
	[TestMethod]
	public void TestDeserialization_FullObject() {
		string json =
			@"{""RawId"": ""ff66358b8ef2e071e4e387b692bcdf019e112574ee5cc4678af35e0cb89b2416"", ""Properties"": {""1"": {""ItemTypeId"": 159}, ""2"": {""Affixes"": []}, ""3"": {""X"": 0, ""Y"": 2, ""Price"": {""Kind"": ""None"", ""Amount"": 0, ""Currency"": ""None""}}, ""4"": {""Name"": """", ""Flags"": ""Corrupted, Fractured"", ""ItemLevel"": 86, ""Influences"": """"}}}";
		var item = JsonSerializer.Deserialize<LoggedItem>(json, new JsonSerializerOptions() {
			Converters = {
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)
			}
		});
		Assert.AreEqual("ff66358b8ef2e071e4e387b692bcdf019e112574ee5cc4678af35e0cb89b2416", item.RawId);
		Assert.AreEqual(4, item.Properties.Count);
		Assert.AreEqual(159, ((ItemTypeAnalysis)item.Properties[KnownAnalyzers.ItemType]).ItemTypeId);
		Assert.AreEqual(ItemFlags.Corrupted | ItemFlags.Fractured, ((ItemDetailsAnalysis)item.Properties[KnownAnalyzers.ItemDetails]).Flags);
	}

	[TestMethod]
	public void TestDeserialization_Array() {
		string json =
			@"[{""RawId"": ""3e51a36c12b7cad75756e51aaf70d7cedae724bb62bb299cd663ceb52c39be6e"", ""Properties"": {""1"": {""ItemTypeId"": 1582}, ""2"": {""Affixes"": [{""hash"": 10193002890527249144, ""kind"": ""Explicit"", ""scalar"": 0}]}, ""3"": {""X"": 0, ""Y"": 1, ""Price"": {""Kind"": ""None"", ""Amount"": 0, ""Currency"": ""None""}}, ""4"": {""Name"": ""Hall of Grandmasters"", ""Flags"": """", ""ItemLevel"": 83, ""Influences"": """"}}}, {""RawId"": ""920aa54d45b96bf565496e65e71f6dae2bce69327953e7ce204f92b05074895e"", ""Properties"": {""1"": {""ItemTypeId"": 2035}, ""2"": {""Affixes"": [{""hash"": 5043307326315620280, ""kind"": ""Explicit"", ""scalar"": 29}, {""hash"": 7093063654257138333, ""kind"": ""Explicit"", ""scalar"": 26}, {""hash"": 3713226157940806579, ""kind"": ""Explicit"", ""scalar"": 40}, {""hash"": 13715899977654915872, ""kind"": ""Explicit"", ""scalar"": 6}, {""hash"": 1734230551950259170, ""kind"": ""Explicit"", ""scalar"": 10}, {""hash"": 7009436602697730924, ""kind"": ""Explicit"", ""scalar"": 1}]}, ""3"": {""X"": 0, ""Y"": 4, ""Price"": {""Kind"": ""None"", ""Amount"": 0, ""Currency"": ""None""}}, ""4"": {""Name"": """", ""Flags"": """", ""ItemLevel"": 0, ""Influences"": """"}}}, {""RawId"": ""47584831ae953fb5d528412456f7c3d75258d1c0d07f0e4c5e31ab7b964abab0"", ""Properties"": {""1"": {""ItemTypeId"": 1582}, ""2"": {""Affixes"": [{""hash"": 10193002890527249144, ""kind"": ""Explicit"", ""scalar"": 0}]}, ""3"": {""X"": 0, ""Y"": 0, ""Price"": {""Kind"": ""None"", ""Amount"": 0, ""Currency"": ""None""}}, ""4"": {""Name"": ""Hall of Grandmasters"", ""Flags"": """", ""ItemLevel"": 81, ""Influences"": """"}}}]";
		var items = JsonSerializer.Deserialize<LoggedItem[]>(json, new JsonSerializerOptions() {
			Converters = {
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
			}
		});
		Assert.AreEqual(3, items.Length);

		var first = items.First();
		var affixes = (ItemAffixesAnalysis)first.Properties[KnownAnalyzers.Modifiers];
		
		Assert.AreEqual(1, affixes.Affixes.Length);
		var affix = affixes.Affixes.First();
		
		Assert.AreEqual(new Affix(10193002890527249144, 0, ModKind.Explicit), affix);
	}

	[TestMethod]
	public void TestSerialization_FullObject() {
		var obj = new LoggedItem("abc", new Dictionary<ushort, IAnalyzedProperties>() {
			{4, new ItemDetailsAnalysis("foo", ItemFlags.Corrupted | ItemFlags.Fractured, InfluenceKind.None, 86)}
		});

		string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions() {
			Converters = {
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)
			}
		});

		string expected =
			@"{""RawId"":""abc"",""Properties"":{""4"":{""Name"":""foo"",""Flags"":""Corrupted, Fractured"",""Influences"":""None"",""ItemLevel"":86}}}";
		Assert.AreEqual(expected, json);
	}
}