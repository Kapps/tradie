using SpanJson;
using System;
using System.Runtime.Serialization;

namespace Tradie.Common.RawModels {
	/// <summary>
	/// Raw information about an item, containing all of the raw data provided by the stash tab API.
	/// </summary>
	/// <remarks>
	/// See https://app.swaggerhub.com/apis-docs/Chuanhsing/poe/1.0.0 for information about undocumented properties.
	/// </remarks>
	public record Item {
        /// <summary>
		/// Unique identifier for this item; usually a 64 character string.
		/// </summary>
        [DataMember(Name = "id")]
        public readonly string Id;
        /// <summary>
		/// Whether this item has been verified as really existing in-game.
		/// </summary>
        [DataMember(Name = "verified")]
        public readonly bool Verified;
        /// <summary>
		/// Number of inventory slots this item takes up horizontally.
		/// </summary>
		[DataMember(Name = "w")]
        public readonly ushort Width;
		/// <summary>
		/// Number of inventory slots this item takes up vertically.
		/// </summary>
		[DataMember(Name = "h")]
		public readonly ushort Height;
		/// <summary>
		/// X coordinate within the stash tab for the top left of this item, from the top left of the tab.
		/// </summary>
		[DataMember(Name = "x")]
		public readonly ushort X;
		/// <summary>
		/// Y coordinate within the stash tab for the top left of this item, from the top left of the tab.
		/// </summary>
		[DataMember(Name = "y")]
		public readonly ushort Y;
		/// <summary>
		/// Path to the icon for this item, hosted on the PoE CDN.
		/// </summary>
		[DataMember(Name = "icon")]
		public readonly string IconPath;
		/// <summary>
		/// Indicates if this item is a support gem.
		/// </summary>
		[DataMember(Name = "support")]
		public readonly bool Support;
		/// <summary>
		/// Stack count of this item.
		/// </summary>
		[DataMember(Name = "stackSize")]
		public readonly int? StackSize;
		/// <summary>
		/// Maximum tack count of this item.
		/// </summary>
		[DataMember(Name = "maxStackSize")]
		public readonly int? MaxStackSize;
        /// <summary>
		/// Human readable text for the stack size.
		/// </summary>
		[DataMember(Name = "stackSizeText")]
        public readonly string? StackSizeText;
        /// <summary>
		/// Name of the league this item is in.
		/// </summary>
        [DataMember(Name = "league")]
        public readonly string League;
        /// <summary>
		/// Name of the item, such as the random name on a rare item.
		/// </summary>
        [DataMember(Name = "name")]
        public readonly string Name;
        /// <summary>
		/// Includes the item type and possibly modifier names, such as Chain Belt of the Troll.
		/// </summary>
        [DataMember(Name = "typeLine")]
        public readonly string TypeLine;
        /// <summary>
		/// Name of the item type, such as Chain Belt.
		/// </summary>
        [DataMember(Name = "baseType")]
        public readonly string BaseType;
        /// <summary>
		/// Whether this item is either identified or does not need identifying.
		/// </summary>
        [DataMember(Name = "identified")]
        public readonly bool Identified;
        /// <summary>
		/// Item level of the item, if applicable.
		/// </summary>
        [DataMember(Name = "itemLevel")]
        public readonly int? ItemLevel;
        /// <summary>
        /// Deprecated version of ItemLevel that the API sometimes still uses.
        /// If found, the ItemLevel attribute shall be set instead.
        /// </summary>
		[DataMember(Name = "ilvl")]
		[Obsolete("Use ItemLevel property instead.`")]
        public readonly int? Ilvl;
        /// <summary>
		/// Whether this item is locked to the character using it.
		/// </summary>
		[DataMember(Name = "lockedToCharacter")]
        public readonly bool LockedToCharacter;
        /// <summary>
		/// Whether this item is locked to the owning account.
		/// </summary>
		[DataMember(Name = "lockedToAccount")]
        public readonly bool LockedToAccount;
		/// <summary>
		/// The note, usually containing the price.
		/// </summary>
		[DataMember(Name = "note")]
		public readonly string? Note;
		/// <summary>
		/// A note specified on the forum instead of the stash tab API.
		/// </summary>
		[DataMember(Name = "forum_note")]
		public readonly string? ForumNote;
		/// <summary>
		/// Whether the item has been mirrored / duplicated.
		/// </summary>
		[DataMember(Name = "duplicated")]
		public readonly bool Mirrored;
		/// <summary>
		/// Whether the item has already been split.
		/// </summary>
		[DataMember(Name = "split")]
		public readonly bool Split;
		/// <summary>
		/// Whether the item is corrupted, such as with Vaal orbs or krangling.
		/// </summary>
		[DataMember(Name = "corrupted")]
		public readonly bool Corrupted;
		[DataMember(Name = "veiled")]
		public readonly bool Veiled;
		[DataMember(Name = "isRelic")]
		public readonly bool Relic;
		[DataMember(Name = "replica")]
		public readonly bool Replica;

		[DataMember(Name = "synthesized")]
		public readonly bool Synthesized;
		[DataMember(Name = "abyssJewel")]
		public readonly bool AbyssJewel;
		[DataMember(Name = "delve")]
		public readonly bool Delve;
		[DataMember(Name = "fractured")]
		public readonly bool Fractured;

		[DataMember(Name = "talismanTier")]
		public readonly int? TalismanTier;

		[DataMember(Name = "descrText")]
		public readonly string? Description;
		[DataMember(Name = "secDescrText")]
		public readonly string? SecondaryDescription;
		[DataMember(Name = "flavourText")]
		public readonly string[]? FlavourText;
		[DataMember(Name = "flavourTextParsed")]
		public readonly string[]? FlavourTextParsed;

		[DataMember(Name = "influences")]
		public readonly Influence? Influences;

		[DataMember(Name = "utilityMods")]
		public readonly string[]? UtilityMods;
		[DataMember(Name = "enchantMods")]
		public readonly string[]? EnchantMods;
		[DataMember(Name = "scourgeMods")]
		public readonly string[]? ScourgeMods;
		[DataMember(Name = "implicitMods")]
		public readonly string[]? ImplicitMods;
		[DataMember(Name = "explicitMods")]
		public readonly string[]? ExplicitMods;
		[DataMember(Name = "fracturedMods")]
		public readonly string[]? FracturedMods;
		[DataMember(Name = "cosmeticMods")]
		public readonly string[]? CosmeticMods;
		[DataMember(Name = "veiledMods")]
		public readonly string[]? VeiledMods;
		// TODO: logbookMods
		// TODO: ultimatumMods

		[DataMember(Name = "prophecyText")]
		public readonly string? ProphecyText;
		
		// TODO: incubatedItem
		// TODO: scourged
		// TODO: hybrid
		[DataMember(Name = "extended")]
		public ExtendedItemProperties? ExtendedProperties;

		[DataMember(Name = "frameType")]
		public readonly int? FrameType;
		[DataMember(Name = "artFilename")]
		public readonly string? ArtFilename;
		[DataMember(Name = "inventoryId")]
		public readonly string? InventoryId;
		[DataMember(Name = "socket")]
		public readonly int? Socket;
		/// <summary>
		/// Colour of ??? -- S (Strength), D (Dexterity), I (Intelligence), or G (Generic?).
		/// </summary>
		[DataMember(Name = "colour")]
		public readonly string? Colour;

		[DataMember(Name = "sockets")]
		public readonly ItemSocket[]? Sockets;
		[DataMember(Name = "socketedItems")]
		public readonly Item[]? SocketedItems;

        [DataMember(Name = "properties")]
        public readonly ItemProperty[]? Properties;
        [DataMember(Name = "notableProperties")]
        public readonly ItemProperty[]? NotableProperties;
        [DataMember(Name = "additionalProperties")]
        public readonly ItemProperty[]? AdditionalProperties;
        [DataMember(Name = "requirements")]
        public readonly ItemProperty[]? Requirements;
        [DataMember(Name = "nextLevelRequirements")]
        public readonly ItemProperty[]? NextLevelRequirements;

        [JsonConstructor]
        public Item(
			string id, bool verified, ushort width, ushort height, ushort x, ushort y, string iconPath,
			string league, string name, string typeLine, string baseType, bool identified,
			bool support = false, int? stackSize = null, int? maxStackSize = null,
			string? stackSizeText = null, int? itemLevel = null, int? ilvl = null, bool lockedToCharacter = false,
			bool lockedToAccount = default, string? note = null, string? forumNote = null,
			bool mirrored = false, bool split = false, bool corrupted = false, bool veiled = false,
			bool relic = false, bool replica = false, int? talismanTier = null, string? description = null,
			string? secondaryDescription = null, string[]? flavourText = null,
			string[]? flavourTextParsed = null, string[]? utilityMods = null,
			string[]? enchantMods = null, string[]? scourgeMods = null, string[]? implicitMods = null,
			string[]? explicitMods = null, string[]? fracturedMods = null,
			string[]? cosmeticMods = null, string[]? veiledMods = null, string? prophecyText = null,
			int? frameType = default, string? artFilename = null, string? inventoryId = null,
			int? socket = default, string? colour = null, ItemProperty[]? properties = null,
			ItemProperty[]? notableProperties = null, ItemProperty[]? additionalProperties = null,
			ItemProperty[]? requirements = null, ItemProperty[]? nextLevelRequirements = null,
			bool synthesized = false, bool delve = false, bool abyssJewel = false, bool fractured = false,
			Influence? influences = null, ItemSocket[]? sockets = null, Item[]? socketedItems = null,
			ExtendedItemProperties? extendedProperties = null
		) {
	        this.Id = id;
	        this.Verified = verified;
	        this.Width = width;
	        this.Height = height;
	        this.X = x;
	        this.Y = y;
	        this.IconPath = iconPath;
	        this.Support = support;
	        this.StackSize = stackSize;
	        this.MaxStackSize = maxStackSize;
	        this.StackSizeText = stackSizeText;
	        this.League = league;
	        this.Name = name;
	        this.TypeLine = typeLine;
	        this.BaseType = baseType;
	        this.Identified = identified;
	        this.ItemLevel = itemLevel ?? ilvl;
	        //this.Ilvl = this.ItemLevel;
	        this.LockedToCharacter = lockedToCharacter;
	        this.LockedToAccount = lockedToAccount;
	        this.Note = note;
	        this.ForumNote = forumNote;
	        this.Mirrored = mirrored;
	        this.Split = split;
	        this.Corrupted = corrupted;
	        this.Veiled = veiled;
	        this.Relic = relic;
	        this.Replica = replica;
	        this.TalismanTier = talismanTier;
	        this.Description = description;
	        this.SecondaryDescription = secondaryDescription;
	        this.FlavourText = flavourText;
	        this.FlavourTextParsed = flavourTextParsed;
	        this.UtilityMods = utilityMods;
	        this.EnchantMods = enchantMods;
	        this.ScourgeMods = scourgeMods;
	        this.ImplicitMods = implicitMods;
	        this.ExplicitMods = explicitMods;
	        this.FracturedMods = fracturedMods;
	        this.CosmeticMods = cosmeticMods;
	        this.VeiledMods = veiledMods;
	        this.ProphecyText = prophecyText;
	        this.FrameType = frameType;
	        this.ArtFilename = artFilename;
	        this.InventoryId = inventoryId;
	        this.Socket = socket;
	        this.Colour = colour;
	        this.Properties = properties;
	        this.NotableProperties = notableProperties;
	        this.AdditionalProperties = additionalProperties;
	        this.Requirements = requirements;
	        this.NextLevelRequirements = nextLevelRequirements;
	        this.Synthesized = synthesized;
	        this.Delve = delve;
	        this.AbyssJewel = abyssJewel;
	        this.Fractured = fractured;
	        this.Sockets = sockets;
	        this.SocketedItems = socketedItems;
	        this.Influences = influences;
	        this.ExtendedProperties = extendedProperties;
        }
	}
}

