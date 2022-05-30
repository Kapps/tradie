import { appConfig } from "../../app/config";
import { AffixRangeServiceClient } from "../../protos/Services/Web/Proto/AffixRangeServiceServiceClientPb";
import { ListAffixRangesRequest } from "../../protos/Services/Web/Proto/AffixRangeService_pb";
import { memoizePersistent } from "../../utils/cachedResource";
import { AffixRange } from "./affixRange";

export const getAffixRanges = () => {
  return memoizePersistent('affixRanges', async () => {
    const service = new AffixRangeServiceClient(appConfig.apiBaseUrl);
    const request = new ListAffixRangesRequest();
    const response = await service.listAffixRanges(request, null);
    const affixRanges = await response.getAffixrangesList().map(c => AffixRange.fromProto(c));
    return affixRanges;
  });
};
