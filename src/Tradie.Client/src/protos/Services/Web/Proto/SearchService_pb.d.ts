import * as jspb from 'google-protobuf'

import * as Models_Indexer_Proto_SearchQuery_pb from '../../../Models/Indexer/Proto/SearchQuery_pb';
import * as Models_Analyzer_Proto_Item_pb from '../../../Models/Analyzer/Proto/Item_pb';
import * as google_protobuf_timestamp_pb from 'google-protobuf/google/protobuf/timestamp_pb';


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

export class SearchResultEntry extends jspb.Message {
  getItem(): Models_Analyzer_Proto_Item_pb.Item | undefined;
  setItem(value?: Models_Analyzer_Proto_Item_pb.Item): SearchResultEntry;
  hasItem(): boolean;
  clearItem(): SearchResultEntry;

  getFirstseen(): google_protobuf_timestamp_pb.Timestamp | undefined;
  setFirstseen(value?: google_protobuf_timestamp_pb.Timestamp): SearchResultEntry;
  hasFirstseen(): boolean;
  clearFirstseen(): SearchResultEntry;

  getLastcharactername(): string;
  setLastcharactername(value: string): SearchResultEntry;

  getChaosequivalentprice(): number;
  setChaosequivalentprice(value: number): SearchResultEntry;

  getTimesrequested(): number;
  setTimesrequested(value: number): SearchResultEntry;

  getTabname(): string;
  setTabname(value: string): SearchResultEntry;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchResultEntry.AsObject;
  static toObject(includeInstance: boolean, msg: SearchResultEntry): SearchResultEntry.AsObject;
  static serializeBinaryToWriter(message: SearchResultEntry, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchResultEntry;
  static deserializeBinaryFromReader(message: SearchResultEntry, reader: jspb.BinaryReader): SearchResultEntry;
}

export namespace SearchResultEntry {
  export type AsObject = {
    item?: Models_Analyzer_Proto_Item_pb.Item.AsObject,
    firstseen?: google_protobuf_timestamp_pb.Timestamp.AsObject,
    lastcharactername: string,
    chaosequivalentprice: number,
    timesrequested: number,
    tabname: string,
  }
}

export class SearchResponse extends jspb.Message {
  getResultsList(): Array<SearchResultEntry>;
  setResultsList(value: Array<SearchResultEntry>): SearchResponse;
  clearResultsList(): SearchResponse;
  addResults(value?: SearchResultEntry, index?: number): SearchResultEntry;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchResponse.AsObject;
  static toObject(includeInstance: boolean, msg: SearchResponse): SearchResponse.AsObject;
  static serializeBinaryToWriter(message: SearchResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchResponse;
  static deserializeBinaryFromReader(message: SearchResponse, reader: jspb.BinaryReader): SearchResponse;
}

export namespace SearchResponse {
  export type AsObject = {
    resultsList: Array<SearchResultEntry.AsObject>,
  }
}

