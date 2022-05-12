import { appConfig } from "../../app/config";
import { LeagueServiceClient } from "../../protos/Services/Web/Proto/LeagueServiceServiceClientPb";
import { ListLeaguesRequest } from "../../protos/Services/Web/Proto/LeagueService_pb";
import { ModifierServiceClient } from "../../protos/Services/Web/Proto/ModifierServiceServiceClientPb";
import { ListModifiersRequest } from "../../protos/Services/Web/Proto/ModifierService_pb";
import { memoizePersistent } from "../../utils/cachedResource";
import { Modifier } from "./modifier";

export const getModifiers = () => {
  return memoizePersistent('modifiers', async () => {
    const service = new ModifierServiceClient(appConfig.apiBaseUrl);
    const request = new ListModifiersRequest();
    const response = await service.listModifiers(request, null);
    const modifiers = await response.getModifiersList().map(c => Modifier.fromProto(c));
    return modifiers;
  });
};
