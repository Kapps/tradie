using SpanJson;
using System;
using System.Runtime.Serialization;

namespace Tradie.Common.RawModels {
    /// <summary>
	/// Provides information about a single socket present on an item.
	/// </summary>
    public readonly record struct ItemSocket {
		/// <summary>
		/// Which group of links this socket is part of, starting from 0.
		/// Each set of links is a new group.
		/// </summary>
		[DataMember(Name = "group")]
        public readonly int Group;
		/// <summary>
		/// Attribute for the socket (S/D/I/G/A/DV)
		/// </summary>
		[DataMember(Name = "attr")]
        public readonly string? Attribute;
        /// <summary>
		/// Colour of this socket (R/G/B/W/A/DV).
		/// </summary>
		[DataMember(Name = "sColour")]
        public readonly string? Colour;

		[JsonConstructor]
        public ItemSocket(int Group, string? Attribute, string? Colour) {
			this.Group = Group;
			this.Attribute = Attribute;
			this.Colour = Colour;
        }
    }
}
