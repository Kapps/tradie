import { post } from '../../api/apiClient';
import { appConfig } from '../../app/config';
import { ModKey } from '../../protos/Models/Analyzer/Proto/ModifierTypes_pb';
import { AffixRange, SearchGroup, SearchQuery as ProtoSearchQuery, SortOrder } from '../../protos/Models/Indexer/Proto/SearchQuery_pb';
import { SearchServiceClient } from '../../protos/Services/Web/Proto/SearchServiceServiceClientPb';
import { SearchRequest, SearchResponse as SearchResponseProto } from '../../protos/Services/Web/Proto/SearchService_pb';
import { Criteria } from '../criteria/criteria';
import { Item } from '../item/item';
import { SearchQuery } from './search';

export interface SearchResponse {
  results: Item[];
}


export const search = async (query: SearchQuery): Promise<SearchResponse> => {
  const request = new SearchRequest();

  const protoQuery = query.toProto();
  request.setQuery(protoQuery);

  const body = await post('search', request);
  const response = SearchResponseProto.deserializeBinary(body);

  return {
    results: response.getItemsList().map(item => Item.fromProto(item)),
  };
};
