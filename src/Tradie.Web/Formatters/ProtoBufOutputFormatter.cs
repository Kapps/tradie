using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using System.IO.Compression;
using System.Net.Mime;
using Tradie.Common;

namespace Tradie.Web.Formatters;

public class ProtoBufOutputFormatter : IOutputFormatter {
	private BrotliCompressor _compressor;
	public ProtoBufOutputFormatter() {
		this._compressor = new();
	}
	public bool CanWriteResult(OutputFormatterCanWriteContext context) {
		if(context.Object == null)
			return false;
		return context.Object is IMessage;
	}

	public async Task WriteAsync(OutputFormatterWriteContext context) {
		if(context.Object == null)
			throw new FormatException("Output object should be a Protobuf object, not null.");
		
		var msg = (IMessage)context.Object;
		var resp = context.HttpContext.Response;

		// Protobuf library does not support async; ASP.NET Core requires async.
		// TODO: Use RecyclableMemoryStream
		using var ms = new MemoryStream(163840);
		
		await using var compressStream = new BrotliStream(ms, (CompressionLevel)6, true);
		using CodedOutputStream codedOutput = new(compressStream, true);
		msg.WriteTo(codedOutput);
		codedOutput.Flush();
		compressStream.Flush();
		
		ms.Position = 0;
		context.ContentType = "application/protobuf";
		context.ContentTypeIsServerDefined = true;
		resp.Headers.ContentEncoding = new StringValues("br");
		context.HttpContext.Response.ContentLength = ms.Length;
		
		await ms.CopyToAsync(resp.Body);

		/*msg.WriteTo(ms);
		
		ms.Position = 0;
		context.ContentType = "application/protobuf";
		context.ContentTypeIsServerDefined = true;
		await ms.CopyToAsync(resp.Body);*/
	}
}