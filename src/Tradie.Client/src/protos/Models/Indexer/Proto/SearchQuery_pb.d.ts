import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_ModifierTypes_pb from '../../../Models/Analyzer/Proto/ModifierTypes_pb';


export class SearchQuery extends jspb.Message {
  getGroupsList(): Array<SearchGroup>;
  setGroupsList(value: Array<SearchGroup>): SearchQuery;
  clearGroupsList(): SearchQuery;
  addGroups(value?: SearchGroup, index?: number): SearchGroup;

  getSort(): SortOrder | undefined;
  setSort(value?: SortOrder): SearchQuery;
  hasSort(): boolean;
  clearSort(): SearchQuery;

  getLeague(): string;
  setLeague(value: string): SearchQuery;

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
    league: string,
  }
}

export class SearchGroup extends jspb.Message {
  getGroupkind(): number;
  setGroupkind(value: number): SearchGroup;

  getRangesList(): Array<SearchRange>;
  setRangesList(value: Array<SearchRange>): SearchGroup;
  clearRangesList(): SearchGroup;
  addRanges(value?: SearchRange, index?: number): SearchRange;

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
    rangesList: Array<SearchRange.AsObject>,
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

export class SearchRange extends jspb.Message {
  getKey(): Models_Analyzer_Proto_ModifierTypes_pb.ModKey | undefined;
  setKey(value?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey): SearchRange;
  hasKey(): boolean;
  clearKey(): SearchRange;

  getMinvalue(): number;
  setMinvalue(value: number): SearchRange;

  getMaxvalue(): number;
  setMaxvalue(value: number): SearchRange;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchRange.AsObject;
  static toObject(includeInstance: boolean, msg: SearchRange): SearchRange.AsObject;
  static serializeBinaryToWriter(message: SearchRange, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchRange;
  static deserializeBinaryFromReader(message: SearchRange, reader: jspb.BinaryReader): SearchRange;
}

export namespace SearchRange {
  export type AsObject = {
    key?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey.AsObject,
    minvalue: number,
    maxvalue: number,
  }
}

