import { appConfig } from "../../app/config";
import { LeagueServiceClient } from "../../protos/Services/Web/Proto/LeagueServiceServiceClientPb";
import { ListLeaguesRequest } from "../../protos/Services/Web/Proto/LeagueService_pb";
import { memoizeLocalStorage } from "../../utils/cachedResource";

export interface League {
  id: string;
}

export const getActiveLeagues = () => {
  return memoizeLocalStorage('leagues', async () => {
    const service = new LeagueServiceClient(appConfig.apiBaseUrl);
    const request = new ListLeaguesRequest();
    const response = await service.listLeagues(request, null);
    const leagues = await response.getLeaguesList().map(c => <League>c.toObject());
    return leagues;
  });
};
