import * as jspb from 'google-protobuf'



export class ItemPrice extends jspb.Message {
  getCurrency(): number;
  setCurrency(value: number): ItemPrice;

  getAmount(): number;
  setAmount(value: number): ItemPrice;

  getBuyoutkind(): number;
  setBuyoutkind(value: number): ItemPrice;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ItemPrice.AsObject;
  static toObject(includeInstance: boolean, msg: ItemPrice): ItemPrice.AsObject;
  static serializeBinaryToWriter(message: ItemPrice, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ItemPrice;
  static deserializeBinaryFromReader(message: ItemPrice, reader: jspb.BinaryReader): ItemPrice;
}

export namespace ItemPrice {
  export type AsObject = {
    currency: number,
    amount: number,
    buyoutkind: number,
  }
}

export class ItemListingProperties extends jspb.Message {
  getX(): number;
  setX(value: number): ItemListingProperties;

  getY(): number;
  setY(value: number): ItemListingProperties;

  getPrice(): ItemPrice | undefined;
  setPrice(value?: ItemPrice): ItemListingProperties;
  hasPrice(): boolean;
  clearPrice(): ItemListingProperties;

  getNote(): string;
  setNote(value: string): ItemListingProperties;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ItemListingProperties.AsObject;
  static toObject(includeInstance: boolean, msg: ItemListingProperties): ItemListingProperties.AsObject;
  static serializeBinaryToWriter(message: ItemListingProperties, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ItemListingProperties;
  static deserializeBinaryFromReader(message: ItemListingProperties, reader: jspb.BinaryReader): ItemListingProperties;
}

export namespace ItemListingProperties {
  export type AsObject = {
    x: number,
    y: number,
    price?: ItemPrice.AsObject,
    note: string,
  }
}

