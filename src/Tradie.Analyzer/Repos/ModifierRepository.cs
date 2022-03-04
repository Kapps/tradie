using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer.Entities;
using Tradie.Common.RawModels;

namespace Tradie.Analyzer.Repos; 

/// <summary>
/// Repository for storing possible affixes on items.
/// </summary>
public interface IModifierRepository : IAsyncDisposable {
	/// <summary>
	/// Retrieves a list of all possible modifiers.
	/// </summary>
	Task<Modifier[]> RetrieveAll();
	/// <summary>
	/// Retrieves a list of modifiers matching the given hashes.
	/// Hashes not found are not part of the list.
	/// The returned list may be in any order.
	/// </summary>
	Task<Modifier[]> LoadByModHash(params ulong[] hashes);
	/// <summary>
	/// Inserts all of the modifiers with given properties.
	/// If a modifier already exists, an exception is thrown.
	/// </summary>
	Task Insert(IEnumerable<Modifier> baseTypes);
}

public class ModifierDbRepository : IModifierRepository {
	public ModifierDbRepository(AnalysisContext context) {
		this._context = context;
	}
	
	public Task<Modifier[]> RetrieveAll() {
		return this._context.Modifiers.ToArrayAsync();
	}

	public Task<Modifier[]> LoadByModHash(params ulong[] hashes) {
		return this._context.Modifiers.Where(c => hashes.Contains(c.ModHash)).ToArrayAsync();
	}

	public Task Insert(IEnumerable<Modifier> baseTypes) {
		this._context.Modifiers.AddRange(baseTypes);
		return this._context.SaveChangesAsync();
	}
	
	public async ValueTask DisposeAsync() {
		await this._context.DisposeAsync();
	}

	private readonly AnalysisContext _context;
}
