using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Proto;
using Tradie.Analyzer.Repos;
using Tradie.Web.Proto;

namespace Tradie.Web.Services;

public class ItemTypeService : Proto.ItemTypeService.ItemTypeServiceBase {
	public ItemTypeService(AnalysisContext context) {
		this._context = context;
	}
	
	public override async Task<ListItemTypesResponse> ListItemTypes(ListItemTypesRequest request, ServerCallContext context) {
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
					IconUrl = c.IconUrl
				})
			}
		};
	}
	
	private readonly AnalysisContext _context;
}