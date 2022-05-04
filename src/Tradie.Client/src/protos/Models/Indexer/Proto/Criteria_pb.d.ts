import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_Modifier_pb from '../../../Models/Analyzer/Proto/Modifier_pb';
import * as Models_Analyzer_Proto_League_pb from '../../../Models/Analyzer/Proto/League_pb';


export class Criteria extends jspb.Message {
  getId(): string;
  setId(value: string): Criteria;

  getName(): string;
  setName(value: string): Criteria;

  getKind(): CriteriaKind;
  setKind(value: CriteriaKind): Criteria;

  getModifier(): Models_Analyzer_Proto_Modifier_pb.Modifier | undefined;
  setModifier(value?: Models_Analyzer_Proto_Modifier_pb.Modifier): Criteria;
  hasModifier(): boolean;
  clearModifier(): Criteria;

  getLeague(): Models_Analyzer_Proto_League_pb.League | undefined;
  setLeague(value?: Models_Analyzer_Proto_League_pb.League): Criteria;
  hasLeague(): boolean;
  clearLeague(): Criteria;

  getCategory(): string;
  setCategory(value: string): Criteria;

  getSubcategory(): string;
  setSubcategory(value: string): Criteria;

  getUnderlyingCase(): Criteria.UnderlyingCase;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Criteria.AsObject;
  static toObject(includeInstance: boolean, msg: Criteria): Criteria.AsObject;
  static serializeBinaryToWriter(message: Criteria, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Criteria;
  static deserializeBinaryFromReader(message: Criteria, reader: jspb.BinaryReader): Criteria;
}

export namespace Criteria {
  export type AsObject = {
    id: string,
    name: string,
    kind: CriteriaKind,
    modifier?: Models_Analyzer_Proto_Modifier_pb.Modifier.AsObject,
    league?: Models_Analyzer_Proto_League_pb.League.AsObject,
    category: string,
    subcategory: string,
  }

  export enum UnderlyingCase { 
    UNDERLYING_NOT_SET = 0,
    MODIFIER = 10,
    LEAGUE = 11,
    CATEGORY = 12,
    SUBCATEGORY = 13,
  }
}

export enum CriteriaKind { 
  UNKNOWN = 0,
  MODIFIER = 1,
  LEAGUE = 2,
  CATEGORY = 3,
  SUBCATEGORY = 4,
  ITEM_TYPE = 5,
  UNIQUE = 6,
}
