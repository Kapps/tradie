import * as jspb from 'google-protobuf'

import * as Models_Indexer_Proto_Criteria_pb from '../../../Models/Indexer/Proto/Criteria_pb';


export class ListCriteriaRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListCriteriaRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ListCriteriaRequest): ListCriteriaRequest.AsObject;
  static serializeBinaryToWriter(message: ListCriteriaRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListCriteriaRequest;
  static deserializeBinaryFromReader(message: ListCriteriaRequest, reader: jspb.BinaryReader): ListCriteriaRequest;
}

export namespace ListCriteriaRequest {
  export type AsObject = {
  }
}

export class ListCriteriaResponse extends jspb.Message {
  getCriteriasList(): Array<Models_Indexer_Proto_Criteria_pb.Criteria>;
  setCriteriasList(value: Array<Models_Indexer_Proto_Criteria_pb.Criteria>): ListCriteriaResponse;
  clearCriteriasList(): ListCriteriaResponse;
  addCriterias(value?: Models_Indexer_Proto_Criteria_pb.Criteria, index?: number): Models_Indexer_Proto_Criteria_pb.Criteria;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListCriteriaResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ListCriteriaResponse): ListCriteriaResponse.AsObject;
  static serializeBinaryToWriter(message: ListCriteriaResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListCriteriaResponse;
  static deserializeBinaryFromReader(message: ListCriteriaResponse, reader: jspb.BinaryReader): ListCriteriaResponse;
}

export namespace ListCriteriaResponse {
  export type AsObject = {
    criteriasList: Array<Models_Indexer_Proto_Criteria_pb.Criteria.AsObject>,
  }
}

