using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Tradie.Analyzer.Entities; 

/// <summary>
/// Requirements to use a specific item, such as stat or level.
/// </summary>
[Owned]
[DataContract, MessagePackObject]
public class Requirements {
	[Column]
	[DataMember, Key(0)]
	public int Dex;
	[Column]
	[DataMember, Key(1)]
	public int Str;
	[Column]
	[DataMember, Key(2)]
	public int Int;
	[Column]
	[DataMember, Key(3)]
	public int Level;
}