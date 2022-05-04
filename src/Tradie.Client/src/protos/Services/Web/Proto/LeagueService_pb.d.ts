import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_League_pb from '../../../Models/Analyzer/Proto/League_pb';


export class ListLeaguesRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListLeaguesRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ListLeaguesRequest): ListLeaguesRequest.AsObject;
  static serializeBinaryToWriter(message: ListLeaguesRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListLeaguesRequest;
  static deserializeBinaryFromReader(message: ListLeaguesRequest, reader: jspb.BinaryReader): ListLeaguesRequest;
}

export namespace ListLeaguesRequest {
  export type AsObject = {
  }
}

export class ListLeaguesResponse extends jspb.Message {
  getLeaguesList(): Array<Models_Analyzer_Proto_League_pb.League>;
  setLeaguesList(value: Array<Models_Analyzer_Proto_League_pb.League>): ListLeaguesResponse;
  clearLeaguesList(): ListLeaguesResponse;
  addLeagues(value?: Models_Analyzer_Proto_League_pb.League, index?: number): Models_Analyzer_Proto_League_pb.League;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListLeaguesResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ListLeaguesResponse): ListLeaguesResponse.AsObject;
  static serializeBinaryToWriter(message: ListLeaguesResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListLeaguesResponse;
  static deserializeBinaryFromReader(message: ListLeaguesResponse, reader: jspb.BinaryReader): ListLeaguesResponse;
}

export namespace ListLeaguesResponse {
  export type AsObject = {
    leaguesList: Array<Models_Analyzer_Proto_League_pb.League.AsObject>,
  }
}

