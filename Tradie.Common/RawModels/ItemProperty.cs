using SpanJson;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using JsonConstructor = SpanJson.JsonConstructorAttribute;

namespace Tradie.Common.RawModels {
    /// <summary>
	/// A single built-in property on an item, such as the quality of the item.
	/// </summary>
    public readonly record struct ItemProperty {
        /// <summary>
		/// Name of this property, such as Quality.
		/// </summary>
		[DataMember(Name = "name")]
        public readonly string Name;
		/// <summary>
		/// The values for this property, such as 20 or "2/10".
		/// </summary>
		[DataMember(Name = "values")]
		public readonly ItemPropertyValue[] Values;
        /// <summary>
		/// Undocumented integer indicating how to display this item?
		/// </summary>
		[DataMember(Name = "displayMode")]
        public readonly int DisplayMode;
		/// <summary>
		/// Possible values:
		///    * 0 - `name` should be displayed as \`${name}: ${values.join(', ')}\` if 
		///        values.length > 0 otherwise just '${name}'
		///    * 1 - `name` should be displayed as \`${values[0]} ${name}\`
		///    * 2 - `name__ should be display as \`${progressBar(values[0])} ${values[0]}\`
		///        i.e. `name` is not displayed
		///    * 3 - `name` field contains placeholders for the values in
		///            the format of `%\d`. The number nth value in `values` (0-based)
		/// </summary>
		public readonly int Type;

        [JsonConstructor]
        public ItemProperty(string name, ItemPropertyValue[] values, int displayMode, int type = 0) {
            this.Name = name;
            this.Values = values;
            this.DisplayMode = displayMode;
			this.Type = type;
        }
    }

    /// <summary>
	/// Value of an item property, such as the numeric amount of a quality.
	/// </summary>
	[JsonCustomSerializer(typeof(ItemPropertyValueJsonSerializer))]
    public record struct ItemPropertyValue {
        /// <summary>
		/// The raw string value, with the format being dependent on the property.
		/// </summary>
        public readonly string Value;
        /// <summary>
		/// 0 - white (simple) text
        /// 1 - blue(augmented) text
        /// 4 - red(fire damage) text
        /// 5 - blue(cold damage) text
        /// 6 - yellow(lightning damage) text
        /// 7 - red-violet(chaos damage) text
        /// </summary>
        public readonly int DisplayType;

        [JsonConstructor]
        public ItemPropertyValue(string value, int displayType) {
			this.Value = value;
			this.DisplayType = displayType;
		}
	}

	public class ItemPropertyValueJsonSerializer : SpanJson.ICustomJsonFormatter<ItemPropertyValue> {
		public static readonly ItemPropertyValueJsonSerializer Default = new ItemPropertyValueJsonSerializer();

		public object? Arguments { get; set; }

		private void SerializeInternal<TSymbol>(ref JsonWriter<TSymbol> writer, ItemPropertyValue value) where TSymbol : struct {
			writer.WriteBeginArray();
			writer.WriteString(value.Value);
			writer.WriteValueSeparator();
			writer.WriteInt32((int)value.DisplayType);
			writer.WriteEndArray();
		}

		private ItemPropertyValue DeserializeInternal<TSymbol>(ref JsonReader<TSymbol> reader) where TSymbol : struct {
			reader.ReadBeginArrayOrThrow();
			string val = reader.ReadString();
			var token = reader.ReadNextToken();
			if(token == JsonToken.ValueSeparator) {
				reader.SkipNextSegment();
			} else {
				throw new JsonParserException(JsonParserException.ParserError.InvalidSymbol, -1);
			}
			int displayType = reader.ReadInt32();
			reader.ReadEndArrayOrThrow();
			return new ItemPropertyValue(val, displayType);
		}

		public ItemPropertyValue Deserialize(ref JsonReader<byte> reader) {
			return DeserializeInternal(ref reader);
		}

		public ItemPropertyValue Deserialize(ref JsonReader<char> reader) {
			return DeserializeInternal(ref reader);
		}

		public void Serialize(ref JsonWriter<byte> writer, ItemPropertyValue value) {
			SerializeInternal(ref writer, value);
		}

		public void Serialize(ref JsonWriter<char> writer, ItemPropertyValue value) {
			SerializeInternal(ref writer, value);
		}
	}
}

