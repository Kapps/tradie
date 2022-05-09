import * as jspb from 'google-protobuf'

import * as Models_Indexer_Proto_SearchQuery_pb from '../../../Models/Indexer/Proto/SearchQuery_pb';
import * as Models_Analyzer_Proto_Item_pb from '../../../Models/Analyzer/Proto/Item_pb';


export class SearchRequest extends jspb.Message {
  getQuery(): Models_Indexer_Proto_SearchQuery_pb.SearchQuery | undefined;
  setQuery(value?: Models_Indexer_Proto_SearchQuery_pb.SearchQuery): SearchRequest;
  hasQuery(): boolean;
  clearQuery(): SearchRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchRequest.AsObject;
  static toObject(includeInstance: boolean, msg: SearchRequest): SearchRequest.AsObject;
  static serializeBinaryToWriter(message: SearchRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchRequest;
  static deserializeBinaryFromReader(message: SearchRequest, reader: jspb.BinaryReader): SearchRequest;
}

export namespace SearchRequest {
  export type AsObject = {
    query?: Models_Indexer_Proto_SearchQuery_pb.SearchQuery.AsObject,
  }
}

export class SearchResponse extends jspb.Message {
  getItemsList(): Array<Models_Analyzer_Proto_Item_pb.Item>;
  setItemsList(value: Array<Models_Analyzer_Proto_Item_pb.Item>): SearchResponse;
  clearItemsList(): SearchResponse;
  addItems(value?: Models_Analyzer_Proto_Item_pb.Item, index?: number): Models_Analyzer_Proto_Item_pb.Item;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchResponse.AsObject;
  static toObject(includeInstance: boolean, msg: SearchResponse): SearchResponse.AsObject;
  static serializeBinaryToWriter(message: SearchResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchResponse;
  static deserializeBinaryFromReader(message: SearchResponse, reader: jspb.BinaryReader): SearchResponse;
}

export namespace SearchResponse {
  export type AsObject = {
    itemsList: Array<Models_Analyzer_Proto_Item_pb.Item.AsObject>,
  }
}

