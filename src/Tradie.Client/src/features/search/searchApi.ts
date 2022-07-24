import { post } from '../../api/apiClient';
import { SearchRequest, SearchResponse as SearchResponseProto } from '../../protos/Services/Web/Proto/SearchService_pb';
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
