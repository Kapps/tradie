import * as jspb from 'google-protobuf'



export class ItemTypeProperties extends jspb.Message {
  getItemtypeid(): number;
  setItemtypeid(value: number): ItemTypeProperties;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ItemTypeProperties.AsObject;
  static toObject(includeInstance: boolean, msg: ItemTypeProperties): ItemTypeProperties.AsObject;
  static serializeBinaryToWriter(message: ItemTypeProperties, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ItemTypeProperties;
  static deserializeBinaryFromReader(message: ItemTypeProperties, reader: jspb.BinaryReader): ItemTypeProperties;
}

export namespace ItemTypeProperties {
  export type AsObject = {
    itemtypeid: number,
  }
}

