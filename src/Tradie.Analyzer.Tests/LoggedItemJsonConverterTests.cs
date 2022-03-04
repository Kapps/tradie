using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
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
	public void TestSerialization_FullObject() {
		var obj = new LoggedItem("abc", new Dictionary<ushort, IAnalyzedProperties>() {
			{4, new ItemDetailsAnalysis("foo", ItemFlags.Corrupted | ItemFlags.Fractured, InfluenceKind.None, 86)}
		});

		string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions() {
			Converters = {
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)
			}
		});

		Console.WriteLine(json);

		string expected =
			@"{""RawId"":""abc"",""Properties"":{""4"":{""Name"":""foo"",""Flags"":""Corrupted, Fractured"",""Influences"":""None"",""ItemLevel"":86}}}";
		Assert.AreEqual(expected, json);
	}
}