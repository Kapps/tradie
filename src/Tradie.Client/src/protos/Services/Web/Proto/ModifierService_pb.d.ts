import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_Modifier_pb from '../../../Models/Analyzer/Proto/Modifier_pb';


export class ListModifiersRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListModifiersRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ListModifiersRequest): ListModifiersRequest.AsObject;
  static serializeBinaryToWriter(message: ListModifiersRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListModifiersRequest;
  static deserializeBinaryFromReader(message: ListModifiersRequest, reader: jspb.BinaryReader): ListModifiersRequest;
}

export namespace ListModifiersRequest {
  export type AsObject = {
  }
}

export class ListModifiersResponse extends jspb.Message {
  getModifiersList(): Array<Models_Analyzer_Proto_Modifier_pb.Modifier>;
  setModifiersList(value: Array<Models_Analyzer_Proto_Modifier_pb.Modifier>): ListModifiersResponse;
  clearModifiersList(): ListModifiersResponse;
  addModifiers(value?: Models_Analyzer_Proto_Modifier_pb.Modifier, index?: number): Models_Analyzer_Proto_Modifier_pb.Modifier;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListModifiersResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ListModifiersResponse): ListModifiersResponse.AsObject;
  static serializeBinaryToWriter(message: ListModifiersResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListModifiersResponse;
  static deserializeBinaryFromReader(message: ListModifiersResponse, reader: jspb.BinaryReader): ListModifiersResponse;
}

export namespace ListModifiersResponse {
  export type AsObject = {
    modifiersList: Array<Models_Analyzer_Proto_Modifier_pb.Modifier.AsObject>,
  }
}

