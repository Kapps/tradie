import * as jspb from 'google-protobuf'



export class Criteria extends jspb.Message {
  getId(): string;
  setId(value: string): Criteria;

  getName(): string;
  setName(value: string): Criteria;

  getKind(): CriteriaKind;
  setKind(value: CriteriaKind): Criteria;

  getModifierhash(): string;
  setModifierhash(value: string): Criteria;

  getLeague(): string;
  setLeague(value: string): Criteria;

  getCategory(): string;
  setCategory(value: string): Criteria;

  getSubcategory(): string;
  setSubcategory(value: string): Criteria;

  getItemtypeid(): number;
  setItemtypeid(value: number): Criteria;

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
    modifierhash: string,
    league: string,
    category: string,
    subcategory: string,
    itemtypeid: number,
  }

  export enum UnderlyingCase { 
    UNDERLYING_NOT_SET = 0,
    MODIFIERHASH = 10,
    LEAGUE = 11,
    CATEGORY = 12,
    SUBCATEGORY = 13,
    ITEMTYPEID = 14,
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
