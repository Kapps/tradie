using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Tradie.Analyzer.Proto;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Controllers;

[ApiController]
[Route("/modifiers")]
public class ModifierController : IAsyncDisposable {
	public ModifierController(IModifierRepository repository) {
		this._repository = repository;
	}
	
	[HttpGet]
	public async Task<ListModifiersResponse> Get(CancellationToken cancellationToken) {
		var modifiers = await this._repository.RetrieveAll(cancellationToken);
		var response = new ListModifiersResponse() {
			Modifiers = {
				modifiers.Select(c => new Modifier() {
					Hash = (long)c.ModHash, Id = c.Id, Text = c.ModifierText
				})
			},
		};
		return response;
	}

	public ValueTask DisposeAsync() {
		return this._repository.DisposeAsync();
	}

	private readonly IModifierRepository _repository;
	
}