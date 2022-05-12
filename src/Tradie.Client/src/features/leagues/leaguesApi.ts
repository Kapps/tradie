import { appConfig } from "../../app/config";
import { LeagueServiceClient } from "../../protos/Services/Web/Proto/LeagueServiceServiceClientPb";
import { ListLeaguesRequest } from "../../protos/Services/Web/Proto/LeagueService_pb";
import { memoizePersistent } from "../../utils/cachedResource";
import { League } from "./league";


export const getActiveLeagues = () => {
  return memoizePersistent('leagues', async () => {
    const service = new LeagueServiceClient(appConfig.apiBaseUrl);
    const request = new ListLeaguesRequest();
    const response = await service.listLeagues(request, null);
    const leagues = await response.getLeaguesList().map(c => <League>c.toObject());
    return leagues;
  });
};
