using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers.Conversions;

/// <summary>
/// Allows converting from a raw item into a persistent entity, such as an ItemType or Unique.
/// </summary>
public interface IPersistentEntityConverter<T> {
	/// <summary>
	/// Parses out the properties for this entity from the raw item.
	/// </summary>
	T ConvertFromRaw(Item item);

	/// <summary>
	/// Returns whether the persistent entity requires an update based off the properties from an incoming version.
	/// </summary>
	bool RequiresUpdate(T mapped, T incoming);

	/// <summary>
	/// Updates the given existing item with properties from the incoming item when an update is required.
	/// Returns the existing item, modified in place.
	/// </summary>
	T MergeFrom(T existing, T incoming);
}