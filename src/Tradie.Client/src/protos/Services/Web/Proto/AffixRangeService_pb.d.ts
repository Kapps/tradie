import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_AffixRange_pb from '../../../Models/Analyzer/Proto/AffixRange_pb';


export class ListAffixRangesRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListAffixRangesRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ListAffixRangesRequest): ListAffixRangesRequest.AsObject;
  static serializeBinaryToWriter(message: ListAffixRangesRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListAffixRangesRequest;
  static deserializeBinaryFromReader(message: ListAffixRangesRequest, reader: jspb.BinaryReader): ListAffixRangesRequest;
}

export namespace ListAffixRangesRequest {
  export type AsObject = {
  }
}

export class ListAffixRangesResponse extends jspb.Message {
  getAffixrangesList(): Array<Models_Analyzer_Proto_AffixRange_pb.AffixRange>;
  setAffixrangesList(value: Array<Models_Analyzer_Proto_AffixRange_pb.AffixRange>): ListAffixRangesResponse;
  clearAffixrangesList(): ListAffixRangesResponse;
  addAffixranges(value?: Models_Analyzer_Proto_AffixRange_pb.AffixRange, index?: number): Models_Analyzer_Proto_AffixRange_pb.AffixRange;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListAffixRangesResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ListAffixRangesResponse): ListAffixRangesResponse.AsObject;
  static serializeBinaryToWriter(message: ListAffixRangesResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListAffixRangesResponse;
  static deserializeBinaryFromReader(message: ListAffixRangesResponse, reader: jspb.BinaryReader): ListAffixRangesResponse;
}

export namespace ListAffixRangesResponse {
  export type AsObject = {
    affixrangesList: Array<Models_Analyzer_Proto_AffixRange_pb.AffixRange.AsObject>,
  }
}

