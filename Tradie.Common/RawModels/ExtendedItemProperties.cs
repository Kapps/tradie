using SpanJson;
using System.Runtime.Serialization;

namespace Tradie.Common.RawModels; 

/// <summary>
/// Extendable extra information about an item, such as the amount of prefixes or suffixes it has.
/// </summary>
public readonly record struct ExtendedItemProperties {
	[DataMember(Name = "category")]
	public readonly string Category;
	[DataMember(Name = "subcategories")]
	public readonly string[] Subcategories;
	[DataMember(Name = "prefixes")]
	public readonly int? Prefixes;
	[DataMember(Name = "suffixes")]
	public readonly int? Suffixes;

	[JsonConstructor]
	public ExtendedItemProperties(string category, string[] subcategories, int? prefixes, int? suffixes) {
		this.Category = category;
		this.Subcategories = subcategories;
		this.Prefixes = prefixes;
		this.Suffixes = suffixes;
	}
}