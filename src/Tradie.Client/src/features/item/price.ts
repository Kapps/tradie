import { ItemPrice as ProtoItemPrice } from '../../protos/Models/Analyzer/Proto/Properties/ItemListingProperties_pb';
import { BuyoutKind } from './itemProperties';


export enum CurrencyKind {
  None = 0,
  Chaos = 1,
  Exalted = 2,
  Alterations = 3,
  Vaal = 4,
  Fuse = 5,
  Alchemy = 6,
  Gemcutters = 7,
  Chromatics = 8,
  Mirror = 9
}

export class ItemPrice {
  currency: CurrencyKind;
  amount: number;
  buyoutKind: BuyoutKind;

  constructor(currency: CurrencyKind, amount: number, buyoutKind: BuyoutKind) {
    this.currency = currency;
    this.amount = amount;
    this.buyoutKind = buyoutKind;
  }

  toProto(): ProtoItemPrice {
    const proto = new ProtoItemPrice();
    proto.setCurrency(this.currency);
    proto.setAmount(this.amount);
    proto.setBuyoutkind(<number>this.buyoutKind);
    return proto;
  }

  static fromProto(proto: ProtoItemPrice): ItemPrice {
    return new ItemPrice(
      proto.getCurrency(),
      proto.getAmount(),
      <BuyoutKind>proto.getBuyoutkind(),
    );
  }
}
