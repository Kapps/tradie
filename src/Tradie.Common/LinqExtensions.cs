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

	/// <summary>
	/// Returns whether the elements of the two enumerables are equal to one another using the default equality comparer.
	/// Null and empty enumerables are considered to be equal to one another.
	/// </summary>
	public static bool UnorderedSequenceEquals<T>(this IEnumerable<T>? first, IEnumerable<T>? second) where T : IComparable<T> {
		var fa = first?.ToArray() ?? Array.Empty<T>();
		var sa = second?.ToArray() ?? Array.Empty<T>();
		
		if(fa.Length != sa.Length)
			return false;

		return fa.OrderBy(c => c).SequenceEqual(sa.OrderBy(c => c));
	}
}