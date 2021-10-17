using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Common;
using Amazon.S3;
using System.Text;

namespace Tradie.Scanner {
	public class ScannerWorker : BackgroundService {
		private const string paramNextChangeId = "Scanner.NextChangeSetId";

		public ScannerWorker(ILogger<ScannerWorker> logger, IParameterStore paramStore, IApiClient apiClient, IChangeSetParser parser, IChangeSetStore changeSetStore) {
			_logger = logger;
			_paramStore = paramStore;
			_apiClient = apiClient;
			_parser = parser;
			_changeSetStore = changeSetStore;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			var stashParam = await _paramStore.GetParameter(paramNextChangeId, "1295601985-1301161226-1256819523-1404257053-1350867496");
			string nextChangeId = stashParam.Value ?? throw new ArgumentNullException();

			while(!stoppingToken.IsCancellationRequested) {
				try {
					//await Task.Delay(1000, stoppingToken);
					bool slept = false;
					var details = await DispatchNextChangeSet(nextChangeId);
					await _paramStore.SetParameter(paramNextChangeId, details.NextChangeSetId);
					if(details.NextChangeSetId == nextChangeId) {
						slept = true;
						await Task.Delay(1000); // Give the API a bit of time to get something new.
					}
					_logger.LogInformation("Read changeset {changeId} -- next changeset is ${nextChangeId} -- slept: {slept}", nextChangeId, details.NextChangeSetId, slept);
					nextChangeId = details.NextChangeSetId;
				} catch(OperationCanceledException) {
					_logger.LogInformation("Worker cancelled at: {time}", DateTimeOffset.Now);
					return;
				} catch(Exception ex) {
					_logger.LogError("Exception occurred while scanning: {ex}", ex.ToString());
					throw;
				}
			}
		}

		protected async Task<ChangeSetDetails> DispatchNextChangeSet(string changeId) {
			var changeContents = await _apiClient.Get("public-stash-tabs", ("id", changeId));

			using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(changeContents));
			var header = await _parser.ReadHeader(inputStream);

			using var contentStream = new MemoryStream();
			await _parser.ReadChanges(inputStream, contentStream);

			await _changeSetStore.WriteChangeSet(changeId, contentStream.ToArray());
			return header;
		}

		private readonly ILogger<ScannerWorker> _logger;
		private readonly IParameterStore _paramStore;
		private readonly IApiClient _apiClient;
		private readonly IChangeSetParser _parser;
		private readonly IChangeSetStore _changeSetStore;
	}
}