using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Controllers;

[ApiController]
[Route("/affixRanges")]
public class AffixRangeController : IAsyncDisposable {
	public AffixRangeController(AnalysisContext analysisContext) {
		this.analysisContext = analysisContext;
	}
	
	[NonAction]
	public ValueTask DisposeAsync() {
		return this.analysisContext.DisposeAsync();
	}
	
	[HttpGet]
	public async Task<ListAffixRangesResponse> Get() {
		var ranges = await analysisContext.AffixRanges.ToArrayAsync();

		return new ListAffixRangesResponse() {
			AffixRanges = {
				ranges.Select(c=>new Analyzer.Proto.AffixRange() {
					ModHash = (long)c.ModHash,
					ModCategory = (int)c.ModCategory,
					EntityKind = (int)c.EntityKind,
					MinValue = c.MinValue ?? 0,
					MaxValue = c.MaxValue ?? 0
				})
			}
		};
	}

	private readonly AnalysisContext analysisContext;
}