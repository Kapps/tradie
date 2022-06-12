using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Proto;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Controllers;

[ApiController]
[Route("/itemTypes")]
public class ItemTypeController : IAsyncDisposable {
	public ItemTypeController(AnalysisContext context) {
		this._context = context;
	}

	[HttpGet]
	public async Task<ListItemTypesResponse> ListItemTypes() {
		var itemTypes = await this._context.ItemTypes.ToArrayAsync();
		return new ListItemTypesResponse() {
			ItemTypes = {
				itemTypes.Select(c => new ItemType() {
					Category = c.Category,
					Height = c.Height,
					Id = c.Id,
					Width = c.Width,
					Name = c.Name,
					Requirements = c.Requirements == null
						? null
						: new Requirements() {
							Dex = c.Requirements.Dex,
							Int = c.Requirements.Int,
							Str = c.Requirements.Str,
							Level = c.Requirements.Level,
						},
					Subcategories = {
						c.Subcategories
					},
					IconUrl = c.IconUrl ?? "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png"
				})
			}
		};
	}
	
	[NonAction]
	public ValueTask DisposeAsync() {
		return this._context.DisposeAsync();
	}

	private readonly AnalysisContext _context;
}