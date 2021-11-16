using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Analyzers; 

/// <summary>
/// An analyzer that 
/// </summary>
public class ItemTypeAnalyzer : IItemAnalyzer {
	/// <summary>
	/// 
	/// </summary>
	public static Guid Id { get; } = new Guid("6FCA53F9-D3C1-432F-A0CE-9F43EC449C36");

	public Task AnalyzeItems(AnalyzedItem[] items) {
		var baseTypes = items.Select(c => c.RawItem).DistinctBy(c => c.BaseType);
		foreach(var item in items) {
			var raw = item.RawItem;
			Console.WriteLine(@"Skipping item {item}");
		}

		return Task.CompletedTask;
	}
}