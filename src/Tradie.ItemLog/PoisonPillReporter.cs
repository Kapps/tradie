using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Tradie.ItemLog;

/// <summary>
/// Supports reporting of "poison pill" messages that would otherwise block a queue.
/// </summary>
public interface IPoisonPillReporter {
	/// <summary>
	/// Reports that the record at the given offset was unable to be processed with the given details.
	/// </summary>
	Task ReportPoisonedMessage(ItemLogOffset offset, string details, CancellationToken cancellationToken);
}

/// <summary>
/// An IPoisonPillReporter that records to an SQS queue.
/// </summary>
public class SqsPoisonPillReporter : IPoisonPillReporter {
	public SqsPoisonPillReporter(IAmazonSQS sqsClient, string sqsUrl, ILogger<SqsPoisonPillReporter> logger) {
		this._sqsClient = sqsClient;
		this._sqsUrl = sqsUrl;
		this._logger = logger;
	}
	
	public async Task ReportPoisonedMessage(ItemLogOffset offset, string details, CancellationToken cancellationToken) {
		this._logger.LogWarning("Reporting poison pill at offset {Offset} -- details: {Details}", offset, details);
		
		await this._sqsClient.SendMessageAsync(new SendMessageRequest() {
			QueueUrl = this._sqsUrl,
			MessageBody = JsonSerializer.Serialize(new {
				Offset = offset,
				Details = details
			})
		}, cancellationToken);
	}

	private readonly IAmazonSQS _sqsClient;
	private readonly string _sqsUrl;
	private readonly ILogger<IPoisonPillReporter> _logger;
}