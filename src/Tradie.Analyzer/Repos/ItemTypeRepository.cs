using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Repos;

/// <summary>
/// Repository for storing base types for all equippable items.
/// </summary>
public interface IItemTypeRepository {
	/// <summary>
	/// Retrieves a list of all of the base types.
	/// </summary>
	Task<ItemType[]> RetrieveAll();
	/// <summary>
	/// Retrieves a list of item types containing the given names.
	/// Names not found are not part of the list.
	/// The returned list may be in any order.
	/// </summary>
	Task<ItemType[]> LoadByNames(params string[] names);
	/// <summary>
	/// Inserts all of the base types with given properties.
	/// If a type already exists, an exception is thrown.
	/// </summary>
	Task Insert(IEnumerable<ItemType> baseTypes);
}

public class ItemTypeRepository : IItemTypeRepository {
	public ItemTypeRepository(AnalysisContext context) {
		this._context = context;
	}
	
	public Task<ItemType[]> RetrieveAll() {
		return this._context.ItemTypes.ToArrayAsync();
	}

	public Task<ItemType[]> LoadByNames(params string[] names) {
		return this._context.ItemTypes.Where(c => names.Contains(c.Name)).ToArrayAsync();
	}

	public Task Insert(IEnumerable<ItemType> baseTypes) {
		this._context.ItemTypes.AddRange(baseTypes);
		return this._context.SaveChangesAsync();
	}

	private readonly AnalysisContext _context;
}