using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer;

public class AnalyzedPropertyCollectionJsonConverter : JsonConverter<AnalyzedPropertyCollection> {
	public override AnalyzedPropertyCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		// {
		if(reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();

		var propertyCollection = new AnalyzedPropertyCollection();
		// ... }
		while(reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
			// "1": 
			if(reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException();
			ushort analyzerId = ushort.Parse(reader.GetString()!);
			Type analyzerType = KnownAnalyzers.GetTypeForAnalyzer(analyzerId);
			
			// {...}
			IAnalyzedProperties props =
				(IAnalyzedProperties)JsonSerializer.Deserialize(ref reader, analyzerType, options)!;
			propertyCollection.Add(analyzerId, props);
		}

		return propertyCollection;
	}

	public override void Write(Utf8JsonWriter writer, AnalyzedPropertyCollection value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		foreach(var prop in value.Properties) {
			ushort key = KnownAnalyzers.GetAnalyzerIdForType(prop.GetType());
			writer.WritePropertyName(key.ToString());
			JsonSerializer.Serialize(writer, (object)prop, options);
		}
		writer.WriteEndObject();
	}
}