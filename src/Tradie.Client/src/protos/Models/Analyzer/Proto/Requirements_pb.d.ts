import * as jspb from 'google-protobuf'



export class Requirements extends jspb.Message {
  getDex(): number;
  setDex(value: number): Requirements;

  getStr(): number;
  setStr(value: number): Requirements;

  getInt(): number;
  setInt(value: number): Requirements;

  getLevel(): number;
  setLevel(value: number): Requirements;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Requirements.AsObject;
  static toObject(includeInstance: boolean, msg: Requirements): Requirements.AsObject;
  static serializeBinaryToWriter(message: Requirements, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Requirements;
  static deserializeBinaryFromReader(message: Requirements, reader: jspb.BinaryReader): Requirements;
}

export namespace Requirements {
  export type AsObject = {
    dex: number,
    str: number,
    pb_int: number,
    level: number,
  }
}

