/*using MessagePack;
using System.Runtime.Serialization;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Analyzers;

public class UniqueAnalyzer : IItemAnalyzer {
	public ValueTask DisposeAsync() {
		throw new NotImplementedException();
	}

	public ValueTask AnalyzeItems(AnalyzedItem[] items) {
		throw new NotImplementedException();
	}
}

[DataContract, MessagePackObject]
public readonly record struct UniqueItemProperties(
	[property:DataMember, Key(0)] string Name,
	[property:DataMember, Key(1)] int ItemTypeId,
	[property:DataMember, Key(2)] AffixRange Ranges
);*/