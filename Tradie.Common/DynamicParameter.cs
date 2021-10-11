using System;

namespace Tradie.Common {
	/// <summary>
	/// A parameter that is dynamically stored and retrieved from a repository.
	/// </summary>
	public struct DynamicParameter<T> {
		/// <summary>
		/// The unique key of the parameter.
		/// </summary>
		public readonly string Key;
		/// <summary>
		/// The value for the parameter at the time of retrieval.
		/// </summary>
		public readonly T Value;

		/// <summary>
		/// Creates a new parameter with the given not-null key and (allowed to be default) value.
		/// </summary>
		public DynamicParameter(string key, T value) {
			this.Key = key ?? throw new ArgumentNullException(nameof(key));
			this.Value = value;
		}
	}
}