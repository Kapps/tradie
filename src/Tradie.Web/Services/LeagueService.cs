/*using Grpc.Core;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Services;

/// <summary>
/// Grpc controller for allowing retrieval of league information.
/// </summary>
public class LeagueService : Proto.LeagueService.LeagueServiceBase {
	public LeagueService(ILeagueRepository repository) {
		this._repository = repository;
	}
	
	public override async Task<ListLeaguesResponse> ListLeagues(ListLeaguesRequest request, ServerCallContext context) {
		var leagues = await this._repository.GetAll(context.CancellationToken);
		return new ListLeaguesResponse() {
			Leagues = {
				leagues
					.Select(c => new Analyzer.Proto.League() {Id = c.Id})
			}
		};

	}

	private readonly ILeagueRepository _repository;
}*/