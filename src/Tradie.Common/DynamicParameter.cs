namespace Tradie.Common;

/// <summary>
/// A parameter that is dynamically stored and retrieved from a repository.
/// </summary>
/// <param name="Key">The unique key of the parameter.</param>
/// <param name="Value">The value for the parameter at the time of retrieval.</param>
public record struct DynamicParameter<T>(
	string Key,
	T Value
);