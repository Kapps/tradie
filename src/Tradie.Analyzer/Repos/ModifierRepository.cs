using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer.Entities;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Repos; 

/// <summary>
/// Repository for storing possible affixes on items.
/// </summary>
public interface IModifierRepository : IAsyncDisposable, IDisposable {
	/// <summary>
	/// Retrieves a list of all possible modifiers.
	/// </summary>
	Task<Modifier[]> RetrieveAll(CancellationToken cancellationToken = default);
	/// <summary>
	/// Retrieves a list of modifiers matching the given hashes.
	/// Hashes not found are not part of the list.
	/// The returned list may be in any order.
	/// </summary>
	Task<Modifier[]> LoadByModHash(ulong[] hashes, CancellationToken cancellationToken = default);
	/// <summary>
	/// Inserts all of the modifiers with given properties.
	/// If a modifier already exists, an exception is thrown.
	/// </summary>
	Task Insert(IEnumerable<Modifier> baseTypes, CancellationToken cancellationToken = default);
}

public class ModifierDbRepository : IModifierRepository {
	public ModifierDbRepository(AnalysisContext context) {
		this._context = context;
	}
	
	public Task<Modifier[]> RetrieveAll(CancellationToken cancellationToken = default) {
		return this._context.Modifiers.OrderBy(c=>c.Kind).ToArrayAsync(cancellationToken);
	}

	public Task<Modifier[]> LoadByModHash(ulong[] hashes, CancellationToken cancellationToken = default) {
		return this._context.Modifiers.Where(c => hashes.Contains(c.ModHash)).ToArrayAsync(cancellationToken);
	}

	public Task Insert(IEnumerable<Modifier> baseTypes, CancellationToken cancellationToken = default) {
		this._context.Modifiers.AddRange(baseTypes);
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
