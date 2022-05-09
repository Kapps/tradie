import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_ModifierTypes_pb from '../../../../Models/Analyzer/Proto/ModifierTypes_pb';


export class Affix extends jspb.Message {
  getKey(): Models_Analyzer_Proto_ModifierTypes_pb.ModKey | undefined;
  setKey(value?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey): Affix;
  hasKey(): boolean;
  clearKey(): Affix;

  getValue(): number;
  setValue(value: number): Affix;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Affix.AsObject;
  static toObject(includeInstance: boolean, msg: Affix): Affix.AsObject;
  static serializeBinaryToWriter(message: Affix, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Affix;
  static deserializeBinaryFromReader(message: Affix, reader: jspb.BinaryReader): Affix;
}

export namespace Affix {
  export type AsObject = {
    key?: Models_Analyzer_Proto_ModifierTypes_pb.ModKey.AsObject,
    value: number,
  }
}

export class ItemAffixProperties extends jspb.Message {
  getAffixesList(): Array<Affix>;
  setAffixesList(value: Array<Affix>): ItemAffixProperties;
  clearAffixesList(): ItemAffixProperties;
  addAffixes(value?: Affix, index?: number): Affix;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ItemAffixProperties.AsObject;
  static toObject(includeInstance: boolean, msg: ItemAffixProperties): ItemAffixProperties.AsObject;
  static serializeBinaryToWriter(message: ItemAffixProperties, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ItemAffixProperties;
  static deserializeBinaryFromReader(message: ItemAffixProperties, reader: jspb.BinaryReader): ItemAffixProperties;
}

export namespace ItemAffixProperties {
  export type AsObject = {
    affixesList: Array<Affix.AsObject>,
  }
}

