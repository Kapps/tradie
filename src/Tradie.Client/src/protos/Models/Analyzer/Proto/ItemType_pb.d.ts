import * as jspb from 'google-protobuf'

import * as Models_Analyzer_Proto_Requirements_pb from '../../../Models/Analyzer/Proto/Requirements_pb';


export class ItemType extends jspb.Message {
  getId(): number;
  setId(value: number): ItemType;

  getName(): string;
  setName(value: string): ItemType;

  getCategory(): string;
  setCategory(value: string): ItemType;

  getSubcategoriesList(): Array<string>;
  setSubcategoriesList(value: Array<string>): ItemType;
  clearSubcategoriesList(): ItemType;
  addSubcategories(value: string, index?: number): ItemType;

  getRequirements(): Models_Analyzer_Proto_Requirements_pb.Requirements | undefined;
  setRequirements(value?: Models_Analyzer_Proto_Requirements_pb.Requirements): ItemType;
  hasRequirements(): boolean;
  clearRequirements(): ItemType;

  getWidth(): number;
  setWidth(value: number): ItemType;

  getHeight(): number;
  setHeight(value: number): ItemType;

  getIconurl(): string;
  setIconurl(value: string): ItemType;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ItemType.AsObject;
  static toObject(includeInstance: boolean, msg: ItemType): ItemType.AsObject;
  static serializeBinaryToWriter(message: ItemType, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ItemType;
  static deserializeBinaryFromReader(message: ItemType, reader: jspb.BinaryReader): ItemType;
}

export namespace ItemType {
  export type AsObject = {
    id: number,
    name: string,
    category: string,
    subcategoriesList: Array<string>,
    requirements?: Models_Analyzer_Proto_Requirements_pb.Requirements.AsObject,
    width: number,
    height: number,
    iconurl: string,
  }
}

