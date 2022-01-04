namespace Tradie.Common;

/// <summary>
/// Enumerable linq extensions utilized by Tradie.
/// </summary>
public static class LinqExtensions {
	/// <summary>
	/// Concats all non-null entries in the list of entries.
	/// Returns an Enumerable that iterates over each of the non-null entries.
	/// If no entries are found or all entries are null, an empty enumerable is returned.
	/// </summary>
	public static IEnumerable<T> ConcatMany<T>(this IEnumerable<T>? coll, params IEnumerable<T>?[] entries) {
		if(coll != null) {
			foreach(var item in coll) {
				yield return item;
			}
		}

		foreach(var entry in entries) {
			if(entry != null) {
				foreach(var item in entry) {
					yield return item;
				}
			}
		}
	}
}