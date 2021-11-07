using SpanJson;
using System;
using System.Runtime.Serialization;

namespace Tradie.Common.RawModels {
    /// <summary>
	/// Represents the influences on an item.
	/// </summary>
    public readonly record struct Influence {
        [JsonConstructor]
        public Influence(
            bool redeemer,
            bool crusader,
            bool shaper,
            bool warlord,
            bool hunter
        ) {
            this.Redeemer = redeemer;
            this.Crusader = crusader;
            this.Shaper = shaper;
            this.Warlord = warlord;
            this.Hunter = hunter;
        }

        [DataMember(Name = "redeemer")]
        public readonly bool Redeemer;

        [DataMember(Name = "crusader")]
        public readonly bool Crusader;

        [DataMember(Name = "shaper")]
        public readonly bool Shaper;

        [DataMember(Name = "warlord")]
        public readonly bool Warlord;

        [DataMember(Name = "hunter")]
        public readonly bool Hunter;
    }
}