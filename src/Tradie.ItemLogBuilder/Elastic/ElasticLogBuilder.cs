using Elasticsearch.Net;
using Nest;
using Tradie.ItemLog;
using Tradie.ItemLogBuilder.Elastic.Models;

namespace Tradie.ItemLogBuilder.Elastic;

public class ElasticLogBuilder : IItemLogBuilder {
	public string Name { get; } = "Elastic";

	public ElasticLogBuilder(IElasticClient elasticClient) {
		this._elasticClient = elasticClient;
	}
	
	public async Task AppendEntries(IAsyncEnumerable<LogRecord> records, CancellationToken cancellationToken = default) {
		var tabs = await records.Select(LoggedTab.FromLogRecord).ToArrayAsync(cancellationToken);
		/*var req = new BulkRequest() {
			Operations = new BulkOperationsCollection<IBulkOperation>(
				tabs.SelectMany(c=>c.Items.Select(d=>new BulkIndexOperation<LoggedItem>(d)))
			)
		};*/

		var semaphore = new SemaphoreSlim(0);
		Exception? error = null;

		var req = new BulkAllRequest<LoggedTab>(tabs) {
			BackOffRetries = 3,
			BackOffTime = TimeSpan.FromSeconds(5),
			RefreshOnCompleted = true,
			MaxDegreeOfParallelism = 4,
			Size = 1000
		};
		using var resp = this._elasticClient.BulkAll(req);
		
		resp.Subscribe(new BulkAllObserver(
			onNext: (b) => Console.WriteLine($"Indexed {b.Items.Count} tabs."),
			onError: (e) => {
				Console.WriteLine($"Error: {e.Message}");
				error = e;
				semaphore.Release();
			},
			onCompleted: () => semaphore.Release()
		));
		
		await semaphore.WaitAsync(cancellationToken);
		if(error != null) throw error;

		/*Console.WriteLine(resp.DebugInformation);
		Console.WriteLine(SpanJson.JsonSerializer.Generic.Utf16.Serialize(resp.Items));*/
	}


	private IElasticClient _elasticClient;
}