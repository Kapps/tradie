using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using SpanJson;
using Tradie.Analyzer.Proto;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Services;

public class CriteriaService : Proto.CriteriaService.CriteriaServiceBase {
	private string CacheKey = "Tradie:Web:Criteria:CriteriaCache";
	
	public CriteriaService(ILeagueRepository leagueRepository, IModifierRepository modifierRepository,
		IItemTypeRepository itemTypeRepository, IDistributedCache cache) {
		
		this._leagueRepository = leagueRepository;
		this._modifierRepository = modifierRepository;
		this._itemTypeRepository = itemTypeRepository;
		this._cache = cache;
	}
	public override async Task<ListCriteriaResponse> ListCriteria(ListCriteriaRequest request, ServerCallContext context) {
		var res = new ListCriteriaResponse();
		IEnumerable<Criteria> criteria;
		if(await this._cache.GetAsync(this.CacheKey) is var cacheBytes && cacheBytes is not null) {
			criteria = JsonSerializer.Generic.Utf8.Deserialize<IEnumerable<Criteria>>(cacheBytes);
		} else {
			criteria = await GenerateCriteria(context.CancellationToken);
			byte[] bytes = JsonSerializer.Generic.Utf8.Serialize((Criteria[])criteria);
			await this._cache.SetAsync(this.CacheKey, bytes, new DistributedCacheEntryOptions() {
				AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
			}, context.CancellationToken);
		}
		res.Criterias.AddRange(criteria);
		return res;
	}

	private async Task<Criteria[]> GenerateCriteria(CancellationToken token) {
		
		var leagues = await this._leagueRepository.GetAll(token);
		var mods = await this._modifierRepository.RetrieveAll(token);
		var itemTypes = await this._itemTypeRepository.RetrieveAll(token);

		var criteria = leagues.Select(c => new Criteria() {
			Id = $"league-{c.Id}",
			Name = c.Id,
			Kind = CriteriaKind.League,
			League = new League() {
				Id = c.Id
			}
		}).Union(itemTypes.Select(c=>c.Category).Where(c=>!String.IsNullOrWhiteSpace(c)).Distinct().Select(c=> new Criteria() {
			Id = $"cat-{c}",
			Name = c,
			Kind = CriteriaKind.Category,
			Category = c
		})).Union(itemTypes.Select(c=>c.Subcategory).Where(c=>!String.IsNullOrWhiteSpace(c)).Distinct().Select(c=> new Criteria() {
			Id = $"subcat-{c}",
			Name = c,
			Kind = CriteriaKind.Subcategory,
			Subcategory = c
		})).Union(mods.Select(c => new Criteria() {
			Id = $"mod-{c.Id}",
			Name = c.ModifierText,
			Kind = CriteriaKind.Modifier,
			Modifier = new Modifier() {
				Hash = (long)c.ModHash,
				Id = c.Id,
				Text = c.ModifierText
			}
		})).ToArray();

		return criteria;
	}

	private readonly IDistributedCache _cache;
	private readonly ILeagueRepository _leagueRepository;
	private readonly IModifierRepository _modifierRepository;
	private readonly IItemTypeRepository _itemTypeRepository;
}