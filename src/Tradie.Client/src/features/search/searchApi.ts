import { appConfig } from '../../app/config';
import { ModKey } from '../../protos/Models/Analyzer/Proto/ModifierTypes_pb';
import { Criteria } from '../../protos/Models/Indexer/Proto/Criteria_pb';
import { SearchServiceClient } from '../../protos/Services/Indexer/Proto/SearchControllerServiceClientPb';
import { AffixRange, SearchGroup, SearchQuery, SearchRequest, SortOrder } from '../../protos/Services/Indexer/Proto/SearchController_pb';
import { CriteriaServiceClient } from '../../protos/Services/Web/Proto/CriteriaServiceServiceClientPb';
import { ListCriteriaRequest } from '../../protos/Services/Web/Proto/CriteriaService_pb';
import { ModifierServiceClient } from '../../protos/Services/Web/Proto/ModifierServiceServiceClientPb';
import { ListModifiersRequest } from '../../protos/Services/Web/Proto/ModifierService_pb';
import { getNRandomElements, getRandomElement } from '../../utils/arrayRandom';

export interface SearchResult {
  id: string;
  name: string;

}

export interface SearchResponse {
  results: SearchResult[];
}

export const search = async (criteria: Criteria.AsObject[]): Promise<SearchResponse> => {
  return {
    results: [],
  };
};

export const doTestSearch = async () => {
  const service = new SearchServiceClient('http://localhost:5000');
  const request = new SearchRequest();
  const query = new SearchQuery();
  const group = new SearchGroup();
  const range = new AffixRange();
  const lifeKey = new ModKey();
  const sort = new SortOrder();
  sort.setSortkind(1);
  lifeKey.setLocation(2);
  // eslint-disable-next-line @typescript-eslint/ban-ts-comment
  // @ts-ignore
  // eslint-disable-next-line @typescript-eslint/no-loss-of-precision
  // 9024037547368883200
  lifeKey.setModifier(<any>'9024037547368883040');
  range.setKey(lifeKey);
  range.setMinvalue(500);
  range.setMaxvalue(Infinity);
  group.addRanges(range);
  group.setGroupkind(1);
  query.addGroups(group);
  query.setSort(sort);
  request.setQuery(query);

  const response = await service.searchGear(request, null);
  console.log(response);
  return response;
};