using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Entities;

/// <summary>
/// Represents a change in price for an item.
/// </summary>
[Table("ItemPriceHistory")]
[Index(nameof(ItemId))]
public class ItemPriceHistory {
	/// <summary>
	/// The unique ID of the item that the price is for.
	/// </summary>
	[Column]
	public string ItemId { get; set; }
	/// <summary>
	/// The timestamp when this price was recorded initially.
	/// </summary>
	[Column(TypeName = "timestamp without time zone")]
	public DateTime RecordedTime { get; set; }
	/// <summary>
	/// The type of currency the item was listed for.
	/// </summary>
	[Column]
	public Currency Currency { get; set; }
	/// <summary>
	/// The amount within the currency, rounded to 2 decimal places.
	/// </summary>
	[Column]
	public float Amount {
		get => _amount;
		set => _amount = (float)Math.Round(value, 2);
	}
	/// <summary>
	/// Indicates the buyout 
	/// </summary>
	[Column]
	public BuyoutKind Kind { get; set; }

	/// <summary>
	/// Returns the price of this item, mapped to an ItemPrice instance.
	/// </summary>
	[NotMapped]
	public ItemPrice Price {
		get => new(this.Currency, this.Amount, this.Kind);
		set {
			this.Currency = value.Currency;
			this.Amount = value.Amount;
			this.Kind = value.Kind;
		}
	}

	public ItemPriceHistory(string itemId, ItemPrice price, DateTime recordedTime) {
		this.ItemId = itemId;
		this.Price = price;
		this.RecordedTime = recordedTime;
	}
	
	public ItemPriceHistory(string itemId, DateTime recordedTime, Currency currency, float amount, BuyoutKind kind) {
		this.ItemId = itemId;
		this.RecordedTime = recordedTime;
		this.Currency = currency;
		this.Amount = amount;
		this.Kind = kind;
	}

	public override string ToString() {
		return $"{ItemId} @ {this.RecordedTime}: {this.Price}";
	}
	
	private float _amount;
}