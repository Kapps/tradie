using Microsoft.Extensions.Logging;
using Tradie.Common;
using Tradie.ItemLog;

namespace Tradie.ItemLogBuilder;

/// <summary>
/// Allows streaming log records from one item log to be used as a source to build another item log. 
/// </summary>
public interface ILogStreamer {
	/// <summary>
	/// Begins streaming records from the appropriate position in the log until the end of the log or when cancelled.
	/// </summary>
	Task CopyItemsFromLog(IItemLog source, IItemLogBuilder logBuilder, CancellationToken cancellationToken);
}

/// <summary>
/// An ILogStreamer implementation that records the starting offset for a particular log builder, resuming from the
/// previous point that it left off.
/// Each set of records is sent to he builder in batches, and position recorded at each batch.
/// </summary>
public class LogStreamer : ILogStreamer {
	public LogStreamer(ILogger<ILogStreamer> logger, IParameterStore parameterStore) {
		this._logger = logger;
		this._parameterStore = parameterStore;
	}
	
	public async Task CopyItemsFromLog(IItemLog source, IItemLogBuilder logBuilder, CancellationToken cancellationToken) {
		string key = GetParameterKey(logBuilder.Name);
		var startingOffset = await this._parameterStore.GetParameter<string>(key);
		this._logger.LogInformation("Commencing streaming from {StartingOffset}.", startingOffset);
		
		var sourceRecords = source.GetItems(new(startingOffset.Value), cancellationToken);
		await foreach(var batch in sourceRecords.BatchByAsync(TradieConfig.LogBuilderBatchSize, cancellationToken)) {
			LogRecord lastRecord = default;
			var trackedBatch = batch.WithCompletionCallback(record => Task.FromResult(lastRecord = record));
			
			await logBuilder.AppendEntries(trackedBatch, cancellationToken);
			
			if(lastRecord != default) {
				await this._parameterStore.SetParameter(key, lastRecord.Offset.Offset);
				this._logger.LogInformation("Updated latest offset to {LastOffset}", lastRecord.Offset.Offset);
			}
		}
	}

	private static string GetParameterKey(string builderName) => $"LogBuilder.{builderName}.Offset"; 
	
	private readonly IParameterStore _parameterStore;
	private readonly ILogger<ILogStreamer> _logger;
}