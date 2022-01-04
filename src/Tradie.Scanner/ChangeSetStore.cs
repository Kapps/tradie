﻿using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Tradie.Common;

namespace Tradie.Scanner {
	/// <summary>
	/// Allows for storing changesets asynchronously.
	/// </summary>
	public interface IChangeSetStore {
		/// <summary>
		/// Writes all of the changesets from the given stream to this store.
		/// The stream is expected to be the appropriate format for the raw changesets.
		/// </summary>
		public Task WriteChangeSet(string changeSetId, byte[] changeSetContents);
	}

	/// <summary>
	/// Implementation of IChangeSetStore that writes changesets to a predefined S3 bucket.
	/// </summary>
	public class S3ChangeSetStore : IChangeSetStore {
		public S3ChangeSetStore(ILogger<S3ChangeSetStore> logger, IAmazonS3 s3Client, ICompressor compressor) {
			_s3Client = s3Client;
			_logger = logger;
			_compressor = compressor;
		}

		public async Task WriteChangeSet(string changeSetId, byte[] changeSetContents) {
			var req = new PutObjectRequest() {
				AutoCloseStream = false,
				DisableMD5Stream = false,
				DisablePayloadSigning = false,
				ContentType = "application/json",
				BucketName = TradieConfig.ChangeSetBucket,
				Key = $"{TradieConfig.RawChangeSetPrefix}{changeSetId}.json.br",
			};

			// S3 requires content length to be known, and we have to write to an intermediate memory stream anyways.
			// So just do it all and use the full MS.

			var sw = Stopwatch.StartNew();
			var compressed = _compressor.Compress(changeSetContents);
			var compressMs = sw.ElapsedMilliseconds;

			await using var ms = new MemoryStream(compressed);
			
			req.InputStream = ms;
			req.Headers.ContentLength = ms.Length;
			req.Headers.ContentType = "application/json";
			req.Headers.ContentEncoding = "br";
				
			var putResp = await _s3Client.PutObjectAsync(req);
			var totalMs = sw.ElapsedMilliseconds;

			_logger.LogInformation("Uploaded changeset {changeSetId} with status code {statusCode} (request {requestId}). Took {compressMS}ms to compress and {totalMS}ms to upload {size}KB.",
				changeSetId, putResp.HttpStatusCode, putResp.ResponseMetadata.RequestId, compressMs, totalMs - compressMs, ms.Length / 1024);
		}

		private readonly IAmazonS3 _s3Client;
		private readonly ILogger<S3ChangeSetStore> _logger;
		private readonly ICompressor _compressor;
	}
} 
