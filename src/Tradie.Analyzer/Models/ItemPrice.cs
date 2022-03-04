using MessagePack;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Tradie.Analyzer.Models;

/// <summary>
/// Represents the listed price of an item.
/// </summary>
/// <param name="Currency">The name of the currency (such as exa or chaos).</param>
/// <param name="Amount">The amount of the currency this item is listed for.</param>
[DataContract, MessagePackObject]
public readonly record struct ItemPrice(
	[property: DataMember, Key(0)] BuyoutCurrency Currency,
	[property: DataMember, Key(1)] float Amount,
	[property: DataMember, Key(2)] BuyoutKind Kind
) {
	
	public static bool TryParse(string? note, out ItemPrice itemPrice) {
		itemPrice = default;
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
			_ => throw new ArgumentOutOfRangeException("buyoutType", matches.Groups[0].Value),
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

	private static readonly Regex PriceRegex = new(@"^~(b\/o|price) (\d+) (\w+?)$", RegexOptions.Compiled);

	private static readonly Dictionary<string, BuyoutCurrency> CurrencyMappings =
		new(StringComparer.InvariantCultureIgnoreCase) {
			{"chaos", BuyoutCurrency.Chaos},
			{"exa", BuyoutCurrency.Exalted},
			{"fuse", BuyoutCurrency.Fuse},
			{"vaal", BuyoutCurrency.Vaal},
			{"alt", BuyoutCurrency.Alterations},
			{"alts", BuyoutCurrency.Alterations},
			{"exalt", BuyoutCurrency.Exalted},
			{"exalts", BuyoutCurrency.Exalted},
			{"exalted", BuyoutCurrency.Exalted}
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

public enum BuyoutCurrency : byte {
	None = 0,
	Chaos = 1,
	Exalted = 2,
	Alterations = 3,
	Vaal = 4,
	Fuse = 5,
}