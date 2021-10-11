using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tradie.Common;

namespace Tradie.Scanner {

	public class ScannerWorker : BackgroundService {
		private readonly ILogger<ScannerWorker> _logger;
		private readonly IParameterStore _paramStore;

		public ScannerWorker(ILogger<ScannerWorker> logger, IParameterStore paramStore) {
			_logger = logger;
			_paramStore = paramStore;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			var stashParam = await _paramStore.GetParameter("StashTabId", "1295601985-1301161226-1256819523-1404257053-1350867496");
			while(!stoppingToken.IsCancellationRequested) {
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
				try {
					await Task.Delay(1000, stoppingToken);
				} catch(OperationCanceledException) {
					return;
				} catch(Exception ex) {
					_logger.LogError(ex.ToString());
					throw;
				}
			}
		}
	}
}