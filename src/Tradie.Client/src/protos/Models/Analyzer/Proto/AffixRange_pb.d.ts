import * as jspb from 'google-protobuf'



export class AffixRange extends jspb.Message {
  getModhash(): string;
  setModhash(value: string): AffixRange;

  getMinvalue(): number;
  setMinvalue(value: number): AffixRange;

  getMaxvalue(): number;
  setMaxvalue(value: number): AffixRange;

  getEntitykind(): number;
  setEntitykind(value: number): AffixRange;

  getModcategory(): number;
  setModcategory(value: number): AffixRange;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AffixRange.AsObject;
  static toObject(includeInstance: boolean, msg: AffixRange): AffixRange.AsObject;
  static serializeBinaryToWriter(message: AffixRange, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AffixRange;
  static deserializeBinaryFromReader(message: AffixRange, reader: jspb.BinaryReader): AffixRange;
}

export namespace AffixRange {
  export type AsObject = {
    modhash: string,
    minvalue: number,
    maxvalue: number,
    entitykind: number,
    modcategory: number,
  }
}

