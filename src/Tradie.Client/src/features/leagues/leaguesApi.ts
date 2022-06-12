import { get } from "../../api/apiClient";
import { appConfig } from "../../app/config";
import { LeagueServiceClient } from "../../protos/Services/Web/Proto/LeagueServiceServiceClientPb";
import { ListLeaguesRequest, ListLeaguesResponse } from "../../protos/Services/Web/Proto/LeagueService_pb";
import { memoizePersistent } from "../../utils/cachedResource";
import { League } from "./league";


export const getActiveLeagues = () => {
  return memoizePersistent('leagues', async () => {
    const request = new ListLeaguesRequest();
    const body = await get('leagues');
    const response = await ListLeaguesResponse.deserializeBinary(body);
    const leagues = await response.getLeaguesList().map(c => <League>c.toObject());
    return leagues;
  });
};
