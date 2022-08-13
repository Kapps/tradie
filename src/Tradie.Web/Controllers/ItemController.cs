using Microsoft.AspNetCore.Mvc;

namespace Tradie.Web.Controllers;

[ApiController]
[Route("/items")]
public class ItemController {

	[HttpGet("{itemId}/whisper")]
	public async Task GetWhisperMessage(string itemId) {
		
	}
}