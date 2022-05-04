import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_ModifierTypes_pb from '../../../Models/Analyzer/Proto/ModifierTypes_pb';


export class SearchRequest extends jspb.Message {
  getQuery(): SearchQuery | undefined;
  setQuery(value?: SearchQuery): SearchRequest;
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
    query?: SearchQuery.AsObject,
  }
}

export class SearchResponse extends jspb.Message {
  getIdsList(): Array<string>;
  setIdsList(value: Array<string>): SearchResponse;
  clearIdsList(): SearchResponse;
  addIds(value: string, index?: number): SearchResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchResponse.AsObject;
  static toObject(includeInstance: boolean, msg: SearchResponse): SearchResponse.AsObject;
  static serializeBinaryToWriter(message: SearchResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchResponse;
  static deserializeBinaryFromReader(message: SearchResponse, reader: jspb.BinaryReader): SearchResponse;
}

export namespace SearchResponse {
  export type AsObject = {
    idsList: Array<string>,
  }
}

export class SearchQuery extends jspb.Message {
  getGroupsList(): Array<SearchGroup>;
  setGroupsList(value: Array<SearchGroup>): SearchQuery;
  clearGroupsList(): SearchQuery;
  addGroups(value?: SearchGroup, index?: number): SearchGroup;

  getSort(): SortOrder | undefined;
  setSort(value?: SortOrder): SearchQuery;
  hasSort(): boolean;
  clearSort(): SearchQuery;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchQuery.AsObject;
  static toObject(includeInstance: boolean, msg: SearchQuery): SearchQuery.AsObject;
  static serializeBinaryToWriter(message: SearchQuery, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchQuery;
  static deserializeBinaryFromReader(message: SearchQuery, reader: jspb.BinaryReader): SearchQuery;
}

export namespace SearchQuery {
  export type AsObject = {
    groupsList: Array<SearchGroup.AsObject>,
    sort?: SortOrder.AsObject,
  }
}

export class SearchGroup extends jspb.Message {
  getGroupkind(): number;
  setGroupkind(value: number): SearchGroup;

  getRangesList(): Array<AffixRange>;
  setRangesList(value: Array<AffixRange>): SearchGroup;
  clearRangesList(): SearchGroup;
  addRanges(value?: AffixRange, index?: number): AffixRange;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchGroup.AsObject;
  static toObject(includeInstance: boolean, msg: SearchGroup): SearchGroup.AsObject;
  static serializeBinaryToWriter(message: SearchGroup, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchGroup;
  static deserializeBinaryFromReader(message: SearchGroup, reader: jspb.BinaryReader): SearchGroup;
}

export namespace SearchGroup {
  export type AsObject = {
    groupkind: number,
    rangesList: Array<AffixRange.AsObject>,
  }
}

export class SortOrder extends jspb.Message {
  getSortkind(): number;
  setSortkind(value: number): SortOrder;

  getModifier(): Models_Analyzer_Proto_ModifierTypes_pb.ModKey | undefined;
  setModifier(value?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey): SortOrder;
  hasModifier(): boolean;
  clearModifier(): SortOrder;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SortOrder.AsObject;
  static toObject(includeInstance: boolean, msg: SortOrder): SortOrder.AsObject;
  static serializeBinaryToWriter(message: SortOrder, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SortOrder;
  static deserializeBinaryFromReader(message: SortOrder, reader: jspb.BinaryReader): SortOrder;
}

export namespace SortOrder {
  export type AsObject = {
    sortkind: number,
    modifier?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey.AsObject,
  }
}

export class AffixRange extends jspb.Message {
  getKey(): Models_Analyzer_Proto_ModifierTypes_pb.ModKey | undefined;
  setKey(value?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey): AffixRange;
  hasKey(): boolean;
  clearKey(): AffixRange;

  getMinvalue(): number;
  setMinvalue(value: number): AffixRange;

  getMaxvalue(): number;
  setMaxvalue(value: number): AffixRange;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AffixRange.AsObject;
  static toObject(includeInstance: boolean, msg: AffixRange): AffixRange.AsObject;
  static serializeBinaryToWriter(message: AffixRange, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AffixRange;
  static deserializeBinaryFromReader(message: AffixRange, reader: jspb.BinaryReader): AffixRange;
}

export namespace AffixRange {
  export type AsObject = {
    key?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey.AsObject,
    minvalue: number,
    maxvalue: number,
  }
}

