using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// Requirements to use a specific item, such as stat or level.
/// </summary>
[Owned]
public class Requirements {
	[Column]
	public int Dex;
	[Column]
	public int Str;
	[Column]
	public int Int;
	[Column]
	public int Level;
}