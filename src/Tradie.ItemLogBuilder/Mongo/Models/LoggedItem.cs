#if false
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Tradie.Analyzer;

namespace Tradie.ItemLogBuilder.Mongo.Models;

/// <summary>
/// Representation of a single item analysis for a Mongo database.
/// </summary>
public record struct LoggedItem(
	[property:BsonId] string ItemId,
	[property:BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)] Dictionary<ushort, IAnalyzedProperties> Properties
);
#endif