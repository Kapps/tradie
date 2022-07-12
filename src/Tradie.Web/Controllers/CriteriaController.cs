using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SpanJson;
using Tradie.Analyzer.Proto;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Controllers;

[ApiController]
[Route("/criteria")]
public class CriteriaController : IAsyncDisposable {
	private string CacheKey = "Tradie:Web:Criteria:CriteriaCache";
	
	public CriteriaController(ILeagueRepository leagueRepository, IModifierRepository modifierRepository,
		IItemTypeRepository itemTypeRepository, IDistributedCache cache) {
		
		this._leagueRepository = leagueRepository;
		this._modifierRepository = modifierRepository;
		this._itemTypeRepository = itemTypeRepository;
		this._cache = cache;
	}
	
	[HttpGet]
	public async Task<ListCriteriaResponse> Get(CancellationToken cancellationToken) {
		var res = new ListCriteriaResponse();
		IEnumerable<Criteria> criteria;
		if(await this._cache.GetAsync(this.CacheKey, cancellationToken) is { } cacheBytes) {
			var deserialized = new List<Criteria>();
			var ms = new MemoryStream(cacheBytes);
			while(ms.Position < ms.Length) {
				var item = new Criteria();
				item.MergeDelimitedFrom(ms);
				deserialized.Add(item);
			}

			criteria = deserialized;
			//criteria = JsonSerializer.Generic.Utf8.Deserialize<IEnumerable<Criteria>>(cacheBytes);
		} else {
			criteria = await GenerateCriteria(CancellationToken.None);
			var ms = new MemoryStream();
			foreach(var item in criteria) {
				item.WriteDelimitedTo(ms);
			}
			//byte[] bytes = JsonSerializer.Generic.Utf8.Serialize((Criteria[])criteria);
			await this._cache.SetAsync(this.CacheKey, ms.ToArray(), new DistributedCacheEntryOptions() {
				AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
			}, CancellationToken.None);
		}
		res.Criterias.AddRange(criteria);
		return res;
	}

	private async Task<Criteria[]> GenerateCriteria(CancellationToken token) {
		Console.WriteLine("Regenerating criteria");
		var leagues = await this._leagueRepository.GetAll(token);
		var mods = await this._modifierRepository.RetrieveAll(token);
		var itemTypes = await this._itemTypeRepository.RetrieveAll(token);

		var criteria = leagues.Select(c => new Criteria() {
			Id = $"league-{c.Id}",
			Name = c.Id,
			Kind = CriteriaKind.League,
			League = c.Id,
			/*League = new League() {
				Id = c.Id
			}*/
		}).Union(mods.Select(c => new Criteria() {
			Id = $"mod-{c.Id}",
			Name = c.ModifierText,
			Kind = CriteriaKind.Modifier,
			Modifier = new() {
				Kind = (int)c.Kind,
				ModifierHash = (long)c.ModHash
			}
			/*Modifier = new Modifier() {
				Hash = (long)c.ModHash,
				Id = c.Id,
				Text = c.ModifierText
			}*/
		})).Union(itemTypes.Select(c=>c.Category).Where(c=>!String.IsNullOrWhiteSpace(c)).Distinct().Select(c=> new Criteria() {
			Id = $"cat-{c}",
			Name = c,
			Kind = CriteriaKind.Category,
			Category = c
		})).Union(itemTypes.SelectMany(c=>c.Subcategories ?? Array.Empty<string>()).Where(c=>!String.IsNullOrWhiteSpace(c)).Distinct().Select(c=> new Criteria() {
			Id = $"subcat-{c}",
			Name = c,
			Kind = CriteriaKind.Subcategory,
			Subcategory = c
		})).Union(itemTypes.Select(c=>new Criteria() {
			Id = $"it-{c}",
			Name = c.Name,
			Kind = CriteriaKind.ItemType,
			ItemTypeId = c.Id
			/*ItemType = new ItemType() {
				Id = c.Id,
				Category = c.Category,
				Height = c.Height,
				Width = c.Width,
				Name = c.Name,
				Requirements = c.Requirements == null ? null : new Requirements() {
					Dex = c.Requirements.Dex,
					Int = c.Requirements.Int,
					Str = c.Requirements.Str,
					Level = c.Requirements.Level
				},
				Subcategories = {
					c.Subcategories
				},
				IconUrl = c.IconUrl
			}*/
		})).ToArray();

		return criteria;
	}
	
	[NonAction]
	public async ValueTask DisposeAsync() {
		await this._leagueRepository.DisposeAsync();
		await this._modifierRepository.DisposeAsync();
		await this._itemTypeRepository.DisposeAsync();
	}

	private readonly IDistributedCache _cache;
	private readonly ILeagueRepository _leagueRepository;
	private readonly IModifierRepository _modifierRepository;
	private readonly IItemTypeRepository _itemTypeRepository;
}