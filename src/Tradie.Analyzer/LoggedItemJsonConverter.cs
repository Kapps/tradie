using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer;

public class LoggedItemJsonConverter : JsonConverter<LoggedItem> {
	public override LoggedItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		// {
		if(reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();
		
		// "RawId":
		if(!reader.Read() || reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "RawId")
			throw new JsonException();
		// "12345", 
		if(!reader.Read() || reader.TokenType != JsonTokenType.String)
			throw new JsonException();
		string rawId = reader.GetString();
		
		// "Properties":
		if(!reader.Read() || reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "Properties")
			throw new JsonException();
		// {
		if(!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();

		var allProps = new Dictionary<ushort, IAnalyzedProperties>();
		// ... }
		while(reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
			// "1": 
			if(reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException();
			ushort analyzerId = ushort.Parse(reader.GetString());
			Type analyzerType = KnownAnalyzers.GetTypeForAnalyzer(analyzerId);
			
			// {...}
			IAnalyzedProperties props =
				(IAnalyzedProperties)JsonSerializer.Deserialize(ref reader, analyzerType, options);
			allProps.Add(analyzerId, props);
		}
		
		// }
		if(!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			throw new JsonException();

		return new LoggedItem(rawId, allProps);
	}

	public override void Write(Utf8JsonWriter writer, LoggedItem value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		writer.WriteString("RawId", value.RawId);
		writer.WritePropertyName("Properties");
		writer.WriteStartObject();
		foreach(var kvp in value.Properties) {
			writer.WritePropertyName(kvp.Key.ToString());
			JsonSerializer.Serialize(writer, (object)kvp.Value, options);
		}
		writer.WriteEndObject();
		writer.WriteEndObject();
	}
}