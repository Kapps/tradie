using Tradie.Analyzer;

namespace Tradie.ItemLogBuilder.Postgres; 

/// <summary>
/// An ItemLogBuilder that stores stash tabs in a table with JSON columns for entries. 
/// </summary>
public class PostgresLogBuilder : IItemLogBuilder {
	public async Task AppendEntries(IAsyncEnumerable<AnalyzedStashTab> stashTabs) {
		await foreach(var tab in stashTabs) {
			
		}
	}
	
	
}