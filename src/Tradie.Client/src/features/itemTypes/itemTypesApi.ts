import { get } from "../../api/apiClient";
import { appConfig } from "../../app/config";
import { ItemTypeServiceClient } from "../../protos/Services/Web/Proto/ItemTypeServiceServiceClientPb";
import { ListItemTypesRequest, ListItemTypesResponse } from "../../protos/Services/Web/Proto/ItemTypeService_pb";
import { memoizePersistent } from "../../utils/cachedResource";
import { ItemType } from "./itemType";


export const getItemTypes = async () => {
  return memoizePersistent('itemTypes', async () => {
    const request = new ListItemTypesRequest();
    const body = await get('itemTypes');
    const response = await ListItemTypesResponse.deserializeBinary(body);
    const itemTypes = await response.getItemtypesList().map(c => ItemType.fromProto(c));
    return itemTypes;
  });
};
