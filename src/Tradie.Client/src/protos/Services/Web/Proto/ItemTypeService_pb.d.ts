import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_ItemType_pb from '../../../Models/Analyzer/Proto/ItemType_pb';


export class ListItemTypesRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListItemTypesRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ListItemTypesRequest): ListItemTypesRequest.AsObject;
  static serializeBinaryToWriter(message: ListItemTypesRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListItemTypesRequest;
  static deserializeBinaryFromReader(message: ListItemTypesRequest, reader: jspb.BinaryReader): ListItemTypesRequest;
}

export namespace ListItemTypesRequest {
  export type AsObject = {
  }
}

export class ListItemTypesResponse extends jspb.Message {
  getItemtypesList(): Array<Models_Analyzer_Proto_ItemType_pb.ItemType>;
  setItemtypesList(value: Array<Models_Analyzer_Proto_ItemType_pb.ItemType>): ListItemTypesResponse;
  clearItemtypesList(): ListItemTypesResponse;
  addItemtypes(value?: Models_Analyzer_Proto_ItemType_pb.ItemType, index?: number): Models_Analyzer_Proto_ItemType_pb.ItemType;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListItemTypesResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ListItemTypesResponse): ListItemTypesResponse.AsObject;
  static serializeBinaryToWriter(message: ListItemTypesResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListItemTypesResponse;
  static deserializeBinaryFromReader(message: ListItemTypesResponse, reader: jspb.BinaryReader): ListItemTypesResponse;
}

export namespace ListItemTypesResponse {
  export type AsObject = {
    itemtypesList: Array<Models_Analyzer_Proto_ItemType_pb.ItemType.AsObject>,
  }
}

