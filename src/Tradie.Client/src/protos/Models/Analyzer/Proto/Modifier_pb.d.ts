import * as jspb from 'google-protobuf'



export class Modifier extends jspb.Message {
  getId(): number;
  setId(value: number): Modifier;

  getHash(): string;
  setHash(value: string): Modifier;

  getText(): string;
  setText(value: string): Modifier;

  getKind(): number;
  setKind(value: number): Modifier;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Modifier.AsObject;
  static toObject(includeInstance: boolean, msg: Modifier): Modifier.AsObject;
  static serializeBinaryToWriter(message: Modifier, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Modifier;
  static deserializeBinaryFromReader(message: Modifier, reader: jspb.BinaryReader): Modifier;
}

export namespace Modifier {
  export type AsObject = {
    id: number,
    hash: string,
    text: string,
    kind: number,
  }
}

