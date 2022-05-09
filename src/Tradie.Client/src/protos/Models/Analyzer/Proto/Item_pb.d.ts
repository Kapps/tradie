import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_ItemAnalysis_pb from '../../../Models/Analyzer/Proto/ItemAnalysis_pb';


export class Item extends jspb.Message {
  getRawid(): string;
  setRawid(value: string): Item;

  getPropertiesList(): Array<Models_Analyzer_Proto_ItemAnalysis_pb.ItemAnalysis>;
  setPropertiesList(value: Array<Models_Analyzer_Proto_ItemAnalysis_pb.ItemAnalysis>): Item;
  clearPropertiesList(): Item;
  addProperties(value?: Models_Analyzer_Proto_ItemAnalysis_pb.ItemAnalysis, index?: number): Models_Analyzer_Proto_ItemAnalysis_pb.ItemAnalysis;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Item.AsObject;
  static toObject(includeInstance: boolean, msg: Item): Item.AsObject;
  static serializeBinaryToWriter(message: Item, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Item;
  static deserializeBinaryFromReader(message: Item, reader: jspb.BinaryReader): Item;
}

export namespace Item {
  export type AsObject = {
    rawid: string,
    propertiesList: Array<Models_Analyzer_Proto_ItemAnalysis_pb.ItemAnalysis.AsObject>,
  }
}

