import { appConfig } from "../../app/config";
import { ItemTypeServiceClient } from "../../protos/Services/Web/Proto/ItemTypeServiceServiceClientPb";
import { ListItemTypesRequest } from "../../protos/Services/Web/Proto/ItemTypeService_pb";
import { memoizeLocalStorage } from "../../utils/cachedResource";
import { ItemType } from "./itemType";


export const getItemTypes = () => {
  return memoizeLocalStorage('itemTypes', async () => {
    const service = new ItemTypeServiceClient(appConfig.apiBaseUrl);
    const request = new ListItemTypesRequest();
    const response = await service.listItemTypes(request, null);
    const itemTypes = await response.getItemtypesList().map(c => ItemType.fromProto(c));
    return itemTypes;
  });
};
