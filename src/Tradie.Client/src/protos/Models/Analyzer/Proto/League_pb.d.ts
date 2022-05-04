import * as jspb from 'google-protobuf'



export class League extends jspb.Message {
  getId(): string;
  setId(value: string): League;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): League.AsObject;
  static toObject(includeInstance: boolean, msg: League): League.AsObject;
  static serializeBinaryToWriter(message: League, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): League;
  static deserializeBinaryFromReader(message: League, reader: jspb.BinaryReader): League;
}

export namespace League {
  export type AsObject = {
    id: string,
  }
}

