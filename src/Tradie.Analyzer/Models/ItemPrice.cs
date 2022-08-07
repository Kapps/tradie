using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Tradie.Analyzer.Models;

/// <summary>
/// Represents the listed price of an item.
/// </summary>
/// <param name="Currency">The name of the currency (such as exa or chaos).</param>
/// <param name="Amount">The amount of the currency this item is listed for.</param>
/// <param name="Kind">Whether the price allows negotiation.</param>
[DataContract, MessagePackObject]
public readonly record struct ItemPrice(
	[property: DataMember, Key(0), JsonConverter(typeof(FlagsEnumJsonConverter<Currency>))] Currency Currency,
	[property: DataMember, Key(1)] float Amount,
	[property: DataMember, Key(2), JsonConverter(typeof(FlagsEnumJsonConverter<BuyoutKind>))] BuyoutKind Kind
) {
	/// <summary>
	/// Returns an ItemPrice for items that have no price set.
	/// </summary>
	public static ItemPrice None => new(Currency.None, 0, BuyoutKind.None);

	/// <summary>
	/// Attempts to parse an ItemPrice from a note, such as "~b/o 3.5 exalts".
	/// </summary>
	public static bool TryParse(string? note, out ItemPrice itemPrice) {
		itemPrice = None;
		if(String.IsNullOrWhiteSpace(note)) {
			return false;
		}

		var matches = PriceRegex.Match(note);
		if(!matches.Success) {
			return false;
		}

		if(matches.Groups.Count != 4) {
			return false;
		}

		var buyoutKind = matches.Groups[1].Value switch {
			"b/o" => BuyoutKind.Offer,
			"price" => BuyoutKind.Fixed,
			_ => BuyoutKind.None,
		};

		if(!float.TryParse(matches.Groups[2].ValueSpan, out var price)) {
			// TODO: Handle currency like 3/6 chaos -- remember /0. 
			return false;
		}

		if(!CurrencyMappings.TryGetValue(matches.Groups[3].Value, out var currency)) {
			return false;
		}

		itemPrice = new(currency, price, buyoutKind);
		return true;
	}

	/// <summary>
	/// Attempts to parse a currency value from a trade tag, such as "exalted" to the appropriate Currency.
	/// </summary>
	public static bool TryParseCurrency(string raw, out Currency parsed) {
		return CurrencyMappings.TryGetValue(raw, out parsed);
	}

	private static readonly Regex PriceRegex = new(@"^~(b\/o|price) ([\d\.]+) (\w+?)$", RegexOptions.Compiled);

	private static readonly Dictionary<string, Currency> CurrencyMappings =
		new(StringComparer.InvariantCultureIgnoreCase) {
			{"chaos", Currency.Chaos},
			{"choas", Currency.Chaos},
			{"exa", Currency.Exalted},
			{"fuse", Currency.Fuse},
			{"fusing", Currency.Fuse},
			{"vaal", Currency.Vaal},
			{"alt", Currency.Alterations},
			{"alts", Currency.Alterations},
			{"exalt", Currency.Exalted},
			{"exalts", Currency.Exalted},
			{"exalted", Currency.Exalted},
			{"exalteds", Currency.Exalted},
			{"alch", Currency.Alchemy},
			{"alchs", Currency.Alchemy},
			{"alchemy", Currency.Alchemy},
			{"gcp", Currency.Gemcutters},
			{"gemc", Currency.Gemcutters},
			{"chrom", Currency.Chromatics},
			{"chrome", Currency.Chromatics},
			{"mirror", Currency.Mirror},
			{"kalandra", Currency.Mirror},
			{"mirrors", Currency.Mirror},
			{"mir", Currency.Mirror}
		};
}

/// <summary>
/// Indicates whether a price listed is fixed or negotiable.
/// </summary>
public enum BuyoutKind : byte {
	None = 0,
	/// <summary>
	/// The user is requesting a price as a starting point.
	/// </summary>
	Offer = 1,
	/// <summary>
	/// The price is fixed and negotiation is not accepted.
	/// </summary>
	Fixed = 2
}

public enum Currency : byte {
	None = 0,
	Chaos = 1,
	Exalted = 2,
	Alterations = 3,
	Vaal = 4,
	Fuse = 5,
	Alchemy = 6,
	Gemcutters = 7,
	Chromatics = 8,
	Mirror = 9
}