using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;

namespace Tradie.Analyzer.Models; 

/// <summary>
/// Requirements to use a specific item, such as stat or level.
/// </summary>
[Owned]
public class Requirements {
	[DataMember(Name = "dex")]
	public int Dex;
	[DataMember(Name = "str")]
	public int Str;
	[DataMember(Name = "int")]
	public int Int;
	[DataMember(Name = "level")]
	public int Level;
}