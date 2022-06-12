using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Controllers;

/// <summary>
/// Grpc controller for allowing retrieval of league information.
/// </summary>
[ApiController]
[Route("/leagues")]
public class LeagueController : IAsyncDisposable {
	public LeagueController(ILeagueRepository repository) {
		this._repository = repository;
	}
	
	[HttpGet]
	public async Task<ListLeaguesResponse> Get(CancellationToken cancellationToken) {
		var leagues = await this._repository.GetAll(cancellationToken);
		return new ListLeaguesResponse() {
			Leagues = {
				leagues
					.Select(c => new Analyzer.Proto.League() {Id = c.Id})
			}
		};

	}
	
	public ValueTask DisposeAsync() {
		return this._repository.DisposeAsync();
	}

	private readonly ILeagueRepository _repository;
	
}