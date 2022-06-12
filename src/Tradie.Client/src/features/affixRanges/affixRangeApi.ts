import { get } from "../../api/apiClient";
import { appConfig } from "../../app/config";
//import { AffixRangeServiceClient } from "../../protos/Services/Web/Proto/AffixRangeServiceServiceClientPb";
import { ListAffixRangesRequest, ListAffixRangesResponse } from "../../protos/Services/Web/Proto/AffixRangeService_pb";
import { memoizePersistent } from "../../utils/cachedResource";
import { AffixRange } from "./affixRange";

export const getAffixRanges = () => {
  return memoizePersistent('affixRanges', async () => {
    const request = new ListAffixRangesRequest();
    const responseBody = await get('affixRanges');
    const response = ListAffixRangesResponse.deserializeBinary(responseBody);
    const affixRanges = await response.getAffixrangesList().map(c => AffixRange.fromProto(c));
    return affixRanges;
  });
};
