import * as jspb from 'google-protobuf'



export class ItemDetailProperties extends jspb.Message {
  getName(): string;
  setName(value: string): ItemDetailProperties;

  getFlags(): number;
  setFlags(value: number): ItemDetailProperties;

  getInfluences(): number;
  setInfluences(value: number): ItemDetailProperties;

  getItemlevel(): number;
  setItemlevel(value: number): ItemDetailProperties;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ItemDetailProperties.AsObject;
  static toObject(includeInstance: boolean, msg: ItemDetailProperties): ItemDetailProperties.AsObject;
  static serializeBinaryToWriter(message: ItemDetailProperties, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ItemDetailProperties;
  static deserializeBinaryFromReader(message: ItemDetailProperties, reader: jspb.BinaryReader): ItemDetailProperties;
}

export namespace ItemDetailProperties {
  export type AsObject = {
    name: string,
    flags: number,
    influences: number,
    itemlevel: number,
  }
}

