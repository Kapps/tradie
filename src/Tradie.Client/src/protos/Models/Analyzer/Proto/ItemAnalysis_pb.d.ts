import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_Properties_ItemDetailProperties_pb from '../../../Models/Analyzer/Proto/Properties/ItemDetailProperties_pb';
import * as Models_Analyzer_Proto_Properties_ItemAffixProperties_pb from '../../../Models/Analyzer/Proto/Properties/ItemAffixProperties_pb';
import * as Models_Analyzer_Proto_Properties_ItemListingProperties_pb from '../../../Models/Analyzer/Proto/Properties/ItemListingProperties_pb';
import * as Models_Analyzer_Proto_Properties_ItemTypeProperties_pb from '../../../Models/Analyzer/Proto/Properties/ItemTypeProperties_pb';


export class ItemAnalysis extends jspb.Message {
  getAnalyzerid(): number;
  setAnalyzerid(value: number): ItemAnalysis;

  getBasicproperties(): Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties | undefined;
  setBasicproperties(value?: Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties): ItemAnalysis;
  hasBasicproperties(): boolean;
  clearBasicproperties(): ItemAnalysis;

  getAffixproperties(): Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties | undefined;
  setAffixproperties(value?: Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties): ItemAnalysis;
  hasAffixproperties(): boolean;
  clearAffixproperties(): ItemAnalysis;

  getTradeproperties(): Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties | undefined;
  setTradeproperties(value?: Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties): ItemAnalysis;
  hasTradeproperties(): boolean;
  clearTradeproperties(): ItemAnalysis;

  getTypeproperties(): Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties | undefined;
  setTypeproperties(value?: Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties): ItemAnalysis;
  hasTypeproperties(): boolean;
  clearTypeproperties(): ItemAnalysis;

  getAnalyzerCase(): ItemAnalysis.AnalyzerCase;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ItemAnalysis.AsObject;
  static toObject(includeInstance: boolean, msg: ItemAnalysis): ItemAnalysis.AsObject;
  static serializeBinaryToWriter(message: ItemAnalysis, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ItemAnalysis;
  static deserializeBinaryFromReader(message: ItemAnalysis, reader: jspb.BinaryReader): ItemAnalysis;
}

export namespace ItemAnalysis {
  export type AsObject = {
    analyzerid: number,
    basicproperties?: Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties.AsObject,
    affixproperties?: Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties.AsObject,
    tradeproperties?: Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties.AsObject,
    typeproperties?: Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties.AsObject,
  }

  export enum AnalyzerCase { 
    ANALYZER_NOT_SET = 0,
    BASICPROPERTIES = 2,
    AFFIXPROPERTIES = 3,
    TRADEPROPERTIES = 4,
    TYPEPROPERTIES = 5,
  }
}

