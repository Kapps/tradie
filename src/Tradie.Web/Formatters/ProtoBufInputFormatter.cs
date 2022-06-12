using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Tradie.Web.Formatters;

public class ProtoBufInputFormatter : IInputFormatter {
	public bool CanRead(InputFormatterContext context) {
		if(context.ModelType.GetInterface(nameof(IMessage)) == null)
			return false;
		return true;
	}

	public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context) {
		if(context.ModelType.GetInterface(nameof(IMessage)) == null)
			return await InputFormatterResult.FailureAsync();
		var instance = (IMessage)Activator.CreateInstance(context.ModelType)!;

		using var ms = new MemoryStream();
		await context.HttpContext.Request.Body.CopyToAsync(ms);
		ms.Position = 0;
		
		instance.MergeFrom(ms);
		return await InputFormatterResult.SuccessAsync(instance);
	}
}