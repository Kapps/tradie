using Microsoft.AspNetCore.Mvc;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;
using Tradie.Analyzer.Repos;

namespace Tradie.Web.Controllers;

[ApiController]
[Route("/priceHistory")]
public class PriceHistoryController : IAsyncDisposable {
	public PriceHistoryController(AnalysisContext context, IPriceHistoryRepository priceHistoryRepository) {
		this._context = context;
		this._priceHistoryRepository = priceHistoryRepository;
	}
	
	[HttpGet("{itemId}")]
	public async Task<IEnumerable<ItemPriceHistory>> Get(string itemId) {
		var priceHistory = await this._priceHistoryRepository.LoadPriceHistoryForItem(itemId, CancellationToken.None);
		return priceHistory;
	}

	public async ValueTask DisposeAsync() {
		await _context.DisposeAsync();
		await this._priceHistoryRepository.DisposeAsync();
	}

	private readonly AnalysisContext _context;
	private readonly IPriceHistoryRepository _priceHistoryRepository;
}