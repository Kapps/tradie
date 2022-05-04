import * as jspb from 'google-protobuf'



export class ModKey extends jspb.Message {
  getModifier(): string;
  setModifier(value: string): ModKey;

  getLocation(): number;
  setLocation(value: number): ModKey;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ModKey.AsObject;
  static toObject(includeInstance: boolean, msg: ModKey): ModKey.AsObject;
  static serializeBinaryToWriter(message: ModKey, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ModKey;
  static deserializeBinaryFromReader(message: ModKey, reader: jspb.BinaryReader): ModKey;
}

export namespace ModKey {
  export type AsObject = {
    modifier: string,
    location: number,
  }
}

