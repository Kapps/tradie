using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer;
using Tradie.Analyzer.Entities;

namespace Tradie.Analyzer.Repos;

/// <summary>
/// Repository for storing base types for all equippable items.
/// </summary>
public interface IItemTypeRepository : IAsyncDisposable, IDisposable {
	/// <summary>
	/// Retrieves a list of all of the base types.
	/// </summary>
	Task<ItemType[]> RetrieveAll(CancellationToken cancellationToken);
	/// <summary>
	/// Retrieves a list of item types containing the given names.
	/// Names not found are not part of the list.
	/// The returned list may be in any order.
	/// </summary>
	Task<ItemType[]> LoadByNames(string[] names, CancellationToken cancellationToken);
	/// <summary>
	/// Inserts all of the base types with given properties.
	/// If a type already exists, an exception is thrown.
	/// </summary>
	Task Insert(IEnumerable<ItemType> baseTypes, CancellationToken cancellationToken);
	/// <summary>
	/// Updates all of the given base types with new properties.
	/// </summary>
	Task Update(IEnumerable<ItemType> baseTypes, CancellationToken cancellationToken);
}

public class ItemTypeDbRepository : IItemTypeRepository {
	public ItemTypeDbRepository(AnalysisContext context) {
		this._context = context;
	}
	
	public Task<ItemType[]> RetrieveAll(CancellationToken cancellationToken) {
		return this._context.ItemTypes.ToArrayAsync(cancellationToken);
	}

	public Task<ItemType[]> LoadByNames(string[] names, CancellationToken cancellationToken) {
		return this._context.ItemTypes.Where(c => names.Contains(c.Name)).ToArrayAsync(cancellationToken);
	}

	public Task Insert(IEnumerable<ItemType> baseTypes, CancellationToken cancellationToken) {
		this._context.ItemTypes.AddRange(baseTypes);
		return this._context.SaveChangesAsync(cancellationToken);
	}
	
	public Task Update(IEnumerable<ItemType> baseTypes, CancellationToken cancellationToken) {
		this._context.ItemTypes.UpdateRange(baseTypes);
		return this._context.SaveChangesAsync(cancellationToken);
	}
	
	public async ValueTask DisposeAsync() {
		await this._context.DisposeAsync();
	}
	
	void IDisposable.Dispose() {
		this._context.Dispose();
	}

	private readonly AnalysisContext _context;
}