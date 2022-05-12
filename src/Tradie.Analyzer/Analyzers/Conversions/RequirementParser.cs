using Microsoft.Extensions.Logging;
using Tradie.Analyzer.Entities;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Analyzers.Conversions;

/// <summary>
/// Helper methods for handling requirements.
/// </summary>
public static class RequirementParser {
	
	/// <summary>
	/// Maps the properties on the item to requirements.
	/// </summary>
	public static Requirements? MapRequirements(Item item, bool allowModified) {
		if(item.Requirements == null) {
			return new Requirements();
		}
		
		var res = new Requirements();
		foreach(var prop in item.Requirements) {
			if(prop.Values[0].DisplayType != 0) {
				// TODO: This is going to result in base types having the wrong requirements.
				// Because they're based off item not type.
				// We need to check for DisplayType 0 and only set requirements there.
				// And then for missing requirements, update them with future items.
				// We can do this in the future though.
				//this._logger.LogWarning("Skipping assigning requirements to {Type} with property {Prop} due to being modified", item.BaseType, prop.Name);
				if(!allowModified) {
					return null;
				}
			}
			switch(prop.Name) {
				case "Level":
					res.Level = int.Parse(prop.Values[0].Value);
					break;
				case "Int" :
				case "Intelligence":
					res.Int = int.Parse(prop.Values[0].Value);
					break;
				case "Dex":
				case "Dexterity":
					res.Dex = int.Parse(prop.Values[0].Value);
					break;
				case "Str":
				case "Strength":
					res.Str = int.Parse(prop.Values[0].Value);
					break;
				/*default:
					this._logger.LogWarning("Unknown stat {Stat} on item {Id}", prop.Name, item.Id);
					break;*/
			}
		}
		return res;
	}
}