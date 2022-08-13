/*using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;
using static Tradie.Web.Proto.AffixRangeService;

namespace Tradie.Web.Services;

public class AffixRangeService : AffixRangeServiceBase {
	public AffixRangeService(AnalysisContext analysisContext) {
		this.analysisContext = analysisContext;
	}

	public override async Task<ListAffixRangesResponse> ListAffixRanges(ListAffixRangesRequest request, ServerCallContext context) {
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
*/