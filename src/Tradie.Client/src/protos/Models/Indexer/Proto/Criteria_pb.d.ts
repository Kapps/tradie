import * as jspb from 'google-protobuf'



export class Criteria extends jspb.Message {
  getId(): string;
  setId(value: string): Criteria;

  getName(): string;
  setName(value: string): Criteria;

  getKind(): CriteriaKind;
  setKind(value: CriteriaKind): Criteria;

  getModifier(): ModifierCriteria | undefined;
  setModifier(value?: ModifierCriteria): Criteria;
  hasModifier(): boolean;
  clearModifier(): Criteria;

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
    modifier?: ModifierCriteria.AsObject,
    league: string,
    category: string,
    subcategory: string,
    itemtypeid: number,
  }

  export enum UnderlyingCase { 
    UNDERLYING_NOT_SET = 0,
    MODIFIER = 10,
    LEAGUE = 11,
    CATEGORY = 12,
    SUBCATEGORY = 13,
    ITEMTYPEID = 14,
  }
}

export class ModifierCriteria extends jspb.Message {
  getModifierhash(): string;
  setModifierhash(value: string): ModifierCriteria;

  getKind(): number;
  setKind(value: number): ModifierCriteria;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ModifierCriteria.AsObject;
  static toObject(includeInstance: boolean, msg: ModifierCriteria): ModifierCriteria.AsObject;
  static serializeBinaryToWriter(message: ModifierCriteria, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ModifierCriteria;
  static deserializeBinaryFromReader(message: ModifierCriteria, reader: jspb.BinaryReader): ModifierCriteria;
}

export namespace ModifierCriteria {
  export type AsObject = {
    modifierhash: string,
    kind: number,
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
