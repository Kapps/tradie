import { appConfig } from '../../app/config';
import { CriteriaServiceClient } from '../../protos/Services/Web/Proto/CriteriaServiceServiceClientPb';
import { ListCriteriaRequest } from '../../protos/Services/Web/Proto/CriteriaService_pb';
import { getNRandomElements, getRandomElement } from '../../utils/arrayRandom';
import { memoizePersistent } from '../../utils/cachedResource';
import { Criteria } from './criteria';

const placeholderHints = [
  { hint: 'life > 80' },
  { hint: 'helmet' },
  { hint: '1h' },
  { hint: '2h sword' },
  { hint: 'Goldrim' },
  { hint: 'pdps > 300' },
  { hint: 'crit mult' },
  { hint: '6S' },
  { hint: '6L' },
  { hint: '2R2G2B' },
];

const descriptionHints = [
  { hint: 'You can type in a unique to filter to only the properties the unique has.' },
  { hint: 'After clicking enter to select a modifier, you can type in a value and then start typing another modifier.' },
  { hint: 'Type an item category (such as helmet) to limit the modifier list to only that category.' },
  { hint: 'After selecting a modifier, you can click the badge to limit the values' },
  { hint: 'Have an item you want to use as a base search? Just paste it in the search bar.' },
];

/**
 * Returns count different examples available for filter criteria.
 */
export function getRandomPlaceholderHints(count: number) {
  return getNRandomElements(placeholderHints, count);
}

/**
 * Returns a random longer description indicating what type of criteria can be searched.
 */
export function getRandomDescriptionHint() {
  return getRandomElement(descriptionHints);
}

/**
 * Returns all available filter criteria.
 */
export const getAllCriteria = (): Promise<Criteria[]> => {
  return memoizePersistent('criteria', async () => {
    const service = new CriteriaServiceClient(appConfig.apiBaseUrl);
    const request = new ListCriteriaRequest();
    const response = await service.listCriteria(request, null);
    const criteria = await response.getCriteriasList().map(c => Criteria.fromProto(c));
    return criteria;
  });
};
