using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tradie.Analyzer;

public class FlagsEnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum {
	public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		if(reader.TokenType == JsonTokenType.Number) {
			return (T)Enum.ToObject(typeof(T), reader.GetInt64());
		}
		string? str = reader.GetString();
		if(string.IsNullOrWhiteSpace(str)) {
			return default;
		}

		return Enum.Parse<T>(str);
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		writer.WriteNumberValue(Convert.ToInt64(value));
	}
}