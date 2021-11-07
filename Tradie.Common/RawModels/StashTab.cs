using SpanJson;
using System;
using System.Runtime.Serialization;

namespace Tradie.Common.RawModels {
    /// <summary>
	/// A raw stash tab as returned from the stash tab river.
	/// </summary>
    public readonly record struct RawStashTab {
        /// <summary>
		/// Unique ID of the tab; currently 64 character random string.
		/// </summary>
		[DataMember(Name = "id")]
        public readonly string Id;
        /// <summary>
		/// Whether this tab is public.
		/// Non-public tabs should not be tracked and indicate a deletion or privatization of a possibly previously public tab.
		/// </summary>
        [DataMember(Name = "public")]
        public readonly bool Public;
		/// <summary>
		/// Account name of the owner of the tab.
		/// Can be null, and will be if Public is false.
		/// </summary>
		[DataMember(Name = "accountName")]
		public readonly string? AccountName;
		/// <summary>
		/// The name of the last recorded character the account owning this tab was using.
		/// Can be null, and will be if Public is false.
		/// </summary>
		[DataMember(Name = "lastCharacerName")]
		public readonly string? LastCharacterName;
		/// <summary>
		/// The name of the tab.
		/// Will be null if Public is false, as it's no longer publicly known.
		/// </summary>
		[DataMember(Name = "stash")]
		public readonly string? Name;
		/// <summary>
		/// The type of the stash tab, such as PremiumStash.
		/// </summary>
		[DataMember(Name = "stashType")]
		public readonly string Type;
		/// <summary>
		/// Name of the league, such as Scourge.
		/// Will be null if Public is false.
		/// </summary>
		[DataMember(Name = "league")]
		public readonly string? League;
		/// <summary>
		/// A list of all the items contained in this stash tab.
		/// Will be an empty array if Public is false.
		/// </summary>
		[DataMember(Name = "items")]
		public readonly Item[] Items;

		[JsonConstructor]
		public RawStashTab(string id, bool @public, string? accountName, string? lastCharacterName, string? name, string type, string? league, Item[] items) {
			this.Id = id;
			this.Public = @public;
			this.AccountName = accountName;
			this.LastCharacterName = lastCharacterName;
			this.Name = name;
			this.Type = type;
			this.League = league;
			this.Items = items;
		}
    }
}
