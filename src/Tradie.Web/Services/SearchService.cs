using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Tradie.Analyzer;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Proto;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Web.Proto;

namespace Tradie.Web.Services;

public class SearchService : Proto.SearchService.SearchServiceBase {
	public SearchService(AnalysisContext context) {
		AppContext.SetSwitch(
			"System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
		this._channel = GrpcChannel.ForAddress(TradieConfig.IndexerGrpcAddress, new GrpcChannelOptions() {
			
			HttpHandler = new HttpClientHandler() {
				ServerCertificateCustomValidationCallback = (_, _, _, _) => true
			}
		});
		this._context = context;
	}
	
	public override async Task<SearchResponse> SearchGear(SearchRequest request, ServerCallContext context) {
		var ids = await PerformSearch(request.Query);
		var items = await this._context.LoggedItems.Where(c => ids.Contains(c.RawId)).ToArrayAsync();
		
		var resp = new SearchResponse();
		resp.Items.AddRange(items.Select(FromAnalysis));
		
		return resp;
	}

	private Item FromAnalysis(LoggedItem item) {
		var res = new Item() {
			RawId = item.RawId,
		};
		
		if(item.Properties.Get<IAnalyzedProperties>(KnownAnalyzers.ItemDetails) is ItemDetailsAnalysis details) {
			res.Properties.Add(new Tradie.Analyzer.Proto.ItemAnalysis() {
				AnalyzerId = KnownAnalyzers.ItemDetails,
				BasicProperties = new ItemDetailProperties() {
					Flags = (uint)details.Flags,
					Influences = (uint)details.Influences,
					Name = details.Name,
					ItemLevel = details.ItemLevel.GetValueOrDefault()
				}
			});
		}

		if(item.Properties.Get<IAnalyzedProperties>(KnownAnalyzers.Modifiers) is ItemAffixesAnalysis affixes) {
			var analysis = new Tradie.Analyzer.Proto.ItemAnalysis() {
				AnalyzerId = KnownAnalyzers.Modifiers,
				AffixProperties = new ItemAffixProperties()
			};
			analysis.AffixProperties.Affixes.AddRange(affixes.Affixes.Select(c=>new Affix() {
				Key = new() { Location = (int)c.Kind, Modifier = (long)c.Hash },
				Value = (float)c.Scalar
			}));
			res.Properties.Add(analysis);
		}
		
		if(item.Properties.Get<IAnalyzedProperties>(KnownAnalyzers.ItemType) is ItemTypeAnalysis itemType) {
			res.Properties.Add(new Tradie.Analyzer.Proto.ItemAnalysis() {
				AnalyzerId = KnownAnalyzers.ItemType,
				TypeProperties = new ItemTypeProperties() {
					ItemTypeId = itemType.ItemTypeId
				}
			});
		}
		
		if(item.Properties.Get<IAnalyzedProperties>(KnownAnalyzers.TradeAttributes) is TradeListingAnalysis listing) {
			res.Properties.Add(new Tradie.Analyzer.Proto.ItemAnalysis() {
				AnalyzerId = KnownAnalyzers.TradeAttributes,
				TradeProperties = new ItemListingProperties() {
					X = listing.X, Y = listing.Y,
					Note = listing.Note ?? "",
					Price = listing.Price == null ? null : new() {
						Amount = listing.Price.Value.Amount,
						Currency = (uint)listing.Price.Value.Currency,
						BuyoutKind = (uint)listing.Price.Value.Kind
					}
				}
			});
		}

		return res;
	}

	private async Task<string[]> PerformSearch(Tradie.Indexer.Proto.SearchQuery query) {
		var client = new Indexer.Proto.SearchService.SearchServiceClient(this._channel);
		var request = new Indexer.Proto.SearchRequest() {
			Query = query
		};

		var indexerResponse = await client.SearchGearAsync(request);
		return indexerResponse.Ids.ToArray();
	}

	private GrpcChannel _channel;
	private readonly AnalysisContext _context;
}