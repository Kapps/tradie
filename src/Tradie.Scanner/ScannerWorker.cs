using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Common;
using Amazon.S3;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Tradie.Scanner; 

public class ScannerWorker : BackgroundService {
	private const string ParamNextChangeId = "Scanner.NextChangeSetId";

	public ScannerWorker(ILogger<ScannerWorker> logger, IParameterStore paramStore, IApiClient apiClient, IChangeSetParser parser, IChangeSetStore changeSetStore) {
		_logger = logger;
		_paramStore = paramStore;
		_apiClient = apiClient;
		_parser = parser;
		_changeSetStore = changeSetStore;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		this._logger.LogInformation("Starting scan with build hash {BuildHash}",
			System.Environment.GetEnvironmentVariable("BUILD_HASH"));
		this._logger.LogDebug("Cancellation token is {StoppingToken}", stoppingToken);
			
		var stashParam = await _paramStore.GetParameter(ParamNextChangeId, "1295601985-1301161226-1256819523-1404257053-1350867496");
		string nextChangeId = stashParam.Value ?? throw new ArgumentNullException();

		while(!stoppingToken.IsCancellationRequested) {
			try {
				//await Task.Delay(1000, stoppingToken);
				bool slept = false;
				var details = await DispatchNextChangeSet(nextChangeId);
				await _paramStore.SetParameter(ParamNextChangeId, details.NextChangeSetId);
				if(details.NextChangeSetId == nextChangeId) {
					slept = true;
					await Task.Delay(1000, stoppingToken); // Give the API a bit of time to get something new.
				}
				_logger.LogInformation("Read changeset {ChangeId} -- next changeset is {NextChangeId} -- slept: {Slept}", nextChangeId, details.NextChangeSetId, slept);
				nextChangeId = details.NextChangeSetId;
			} catch(OperationCanceledException ex) {
				_logger.LogInformation("Worker cancelled at: {Time}", DateTimeOffset.Now);
				this._logger.LogWarning("Exception: {Ex} -- stack: {Stack}", ex, ex.StackTrace);
				throw;
			} catch(Exception ex) {
				_logger.LogError("Exception occurred while scanning: {Ex}", ex.ToString());
				throw;
			}
		}
			
		this._logger.LogInformation("We appear to be exiting");
	}

	private async Task<ChangeSetDetails> DispatchNextChangeSet(string changeId) {
		var changeContents = await _apiClient.Get("public-stash-tabs", ("id", changeId));
		/*if(changeContents.Contains("Kapps")) {
			_logger.LogWarning("--------------------Got mine!-----------------");
		}*/

		await using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(changeContents));
		var header = await _parser.ReadHeader(inputStream);

		await using var contentStream = new MemoryStream();
		await _parser.ReadChanges(inputStream, contentStream);

		if(contentStream.Length > 0) {
			// Can get empty changesets if a new one isn't ready yet.
			await _changeSetStore.WriteChangeSet(changeId, contentStream.ToArray());
		}

		return header;
	}

	private readonly ILogger<ScannerWorker> _logger;
	private readonly IParameterStore _paramStore;
	private readonly IApiClient _apiClient;
	private readonly IChangeSetParser _parser;
	private readonly IChangeSetStore _changeSetStore;
}