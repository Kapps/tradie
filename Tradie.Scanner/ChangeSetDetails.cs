using System;
namespace Tradie.Scanner {
	/// <summary>
	/// Provides meta information about a changeset, such as the one following it.
	/// </summary>
	public readonly struct ChangeSetDetails {
		public readonly string NextChangeSetId;

		/// <summary>
		/// Creates a new ChangeSetDetails with the metadata for a changeset.
		/// </summary>
		public ChangeSetDetails(string nextChangeSetId) {
			this.NextChangeSetId = nextChangeSetId;
		}
	}
}
