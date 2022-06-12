import { get } from "../../api/apiClient";
import { appConfig } from "../../app/config";
import { LeagueServiceClient } from "../../protos/Services/Web/Proto/LeagueServiceServiceClientPb";
import { ListLeaguesRequest } from "../../protos/Services/Web/Proto/LeagueService_pb";
import { ModifierServiceClient } from "../../protos/Services/Web/Proto/ModifierServiceServiceClientPb";
import { ListModifiersRequest, ListModifiersResponse } from "../../protos/Services/Web/Proto/ModifierService_pb";
import { memoizePersistent } from "../../utils/cachedResource";
import { Modifier } from "./modifier";

export const getModifiers = () => {
  return memoizePersistent('modifiers', async () => {
    const request = new ListModifiersRequest();
    const body = await get('modifiers');
    const response = ListModifiersResponse.deserializeBinary(body);
    const modifiers = await response.getModifiersList().map(c => Modifier.fromProto(c));
    return modifiers;
  });
};
