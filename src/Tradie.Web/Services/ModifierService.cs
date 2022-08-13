/*using Grpc.Core;
using Tradie.Analyzer.Proto;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Services;

public class ModifierService : Proto.ModifierService.ModifierServiceBase {
	public ModifierService(IModifierRepository repository) {
		this._repository = repository;
	}
	
	public override async Task<ListModifiersResponse> ListModifiers(ListModifiersRequest request, ServerCallContext context) {
		var modifiers = await this._repository.RetrieveAll(context.CancellationToken);
		var response = new ListModifiersResponse() {
			Modifiers = {
				modifiers.Select(c => new Modifier() {
					Hash = (long)c.ModHash, Id = c.Id, Text = c.ModifierText
				})
			},
		};
		return response;
	}

	private readonly IModifierRepository _repository;
}*/