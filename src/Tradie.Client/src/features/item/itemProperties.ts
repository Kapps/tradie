import { ItemPrice } from "./price";

import { ItemTypeProperties as ProtoItemTypeProperties } from '../../protos/Models/Analyzer/Proto/Properties/ItemTypeProperties_pb';
import { ItemDetailProperties as ProtoItemDetailProperties } from '../../protos/Models/Analyzer/Proto/Properties/ItemDetailProperties_pb';
import { ItemListingProperties as ProtoItemListingProperties } from '../../protos/Models/Analyzer/Proto/Properties/ItemListingProperties_pb';
import { Affix as ProtoAffix, ItemAffixProperties as ProtoItemAffixProperties } from '../../protos/Models/Analyzer/Proto/Properties/ItemAffixProperties_pb';
import { ItemAnalysis as ProtoItemAnalysis } from '../../protos/Models/Analyzer/Proto/ItemAnalysis_pb';
import { ModKey } from "../search/search";

export enum BuyoutKind {
  None = 0,
  Offer = 1,
  Fixed = 2
}

export enum Influences {
  None = 0,
  Redeemer = 1,
  Crusader = 2,
  Warlord = 4,
  Hunter = 8,
  Shaper = 16,
  Elder = 32
}

export enum ItemFlags {
  None = 0,
  Corrupted = 1,
  Mirrored = 2,
  Veiled = 4,
  Relic = 8,
  Replica = 16,
  Synthesized = 32,
  Fractured = 64
}

export enum AnalyzerId {
  ItemType = 1,
  Modifiers = 2,
  TradeAttributes = 3,
  ItemDetails = 4,
}

export enum ItemRarity {
  Normal = 0,
  Magic = 1,
  Rare = 2,
  Unique = 3,
  Gem = 4,
  Relic = 9,
}

export class Affix {
  modifier: ModKey;
  value: number;

  constructor(modifier: ModKey, value: number) {
    this.modifier = modifier;
    this.value = value;
  }

  toProto(): ProtoAffix {
    const proto = new ProtoAffix();
    proto.setKey(this.modifier.toProto());
    proto.setValue(this.value);
    return proto;
  }

  static fromProto(proto: ProtoAffix): Affix {
    return new Affix(
      ModKey.fromProto(proto.getKey()!),
      proto.getValue()
    );
  }
}

export class ItemTypeProperties {
  itemTypeId: number;

  constructor(itemTypeId: number) {
    this.itemTypeId = itemTypeId;
  }

  toProto(): ProtoItemTypeProperties {
    const proto = new ProtoItemTypeProperties();
    proto.setItemtypeid(this.itemTypeId);
    return proto;
  }

  static fromProto(proto: ProtoItemTypeProperties): ItemTypeProperties {
    return new ItemTypeProperties(
      proto.getItemtypeid()
    );
  }
}

export class ItemAffixProperties {
  affixes: Affix[];

  constructor(affixes: Affix[]) {
    this.affixes = affixes;
  }

  toProto(): ProtoItemAffixProperties {
    const proto = new ProtoItemAffixProperties();

    proto.addAffixes(...this.affixes.map(a => a.toProto()));
    return proto;
  }

  static fromProto(proto: ProtoItemAffixProperties): ItemAffixProperties {
    return new ItemAffixProperties(
      proto.getAffixesList().map(a => Affix.fromProto(a))
    );
  }
}

export class ItemListingProperties {
  x: number;
  y: number;
  price: ItemPrice;
  note: string;

  constructor(x: number, y: number, price: ItemPrice, note: string) {
    this.x = x;
    this.y = y;
    this.price = price;
    this.note = note;
  }

  toProto(): ProtoItemListingProperties {
    const proto = new ProtoItemListingProperties();
    proto.setX(this.x);
    proto.setY(this.y);
    proto.setPrice(this.price.toProto());
    proto.setNote(this.note);
    return proto;
  }

  static fromProto(proto: ProtoItemListingProperties): ItemListingProperties {
    return new ItemListingProperties(
      proto.getX(),
      proto.getY(),
      ItemPrice.fromProto(proto.getPrice()!),
      proto.getNote()
    );
  }
}

export class ItemDetailProperties {
  name: string;
  flags: ItemFlags;
  influences: Influences;
  itemLevel: number;
  rarity: ItemRarity;

  constructor(name: string, flags: ItemFlags, influences: Influences, itemLevel: number, rarity: ItemRarity) {
    this.name = name;
    this.flags = flags;
    this.influences = influences;
    this.itemLevel = itemLevel;
    this.rarity = rarity;
  }

  toProto(): ProtoItemDetailProperties {
    const proto = new ProtoItemDetailProperties();
    proto.setName(this.name);
    proto.setFlags(this.flags);
    proto.setInfluences(this.influences);
    proto.setItemlevel(this.itemLevel);
    proto.setRarity(this.rarity);

    return proto;
  }

  static fromProto(proto: ProtoItemDetailProperties): ItemDetailProperties {
    return new ItemDetailProperties(
      proto.getName(),
      proto.getFlags(),
      proto.getInfluences(),
      proto.getItemlevel(),
      proto.getRarity(),
    );
  }
}

export class ItemProperties {
  analyzerId: number;
  property: ItemTypeProperties | ItemListingProperties | ItemDetailProperties | ItemAffixProperties;

  constructor(analyzerId: number, property: ItemTypeProperties | ItemListingProperties | ItemDetailProperties | ItemAffixProperties) {
    this.analyzerId = analyzerId;
    this.property = property;
  }

  toProto(): ProtoItemAnalysis {
    const proto = new ProtoItemAnalysis();
    proto.setAnalyzerid(this.analyzerId);
    if (this.property instanceof ItemTypeProperties) {
      proto.setTypeproperties(this.property.toProto());
    } else if (this.property instanceof ItemListingProperties) {
      proto.setTradeproperties(this.property.toProto());
    } else if (this.property instanceof ItemDetailProperties) {
      proto.setBasicproperties(this.property.toProto());
    } else if (this.property instanceof ItemAffixProperties) {
      proto.setAffixproperties(this.property.toProto());
    }
    return proto;
  }

  static fromProto(proto: ProtoItemAnalysis): ItemProperties {
    const getProps = () => {
      if (proto.hasTypeproperties()) {
        return ItemTypeProperties.fromProto(proto.getTypeproperties()!);
      } else if (proto.hasTradeproperties()) {
        return ItemListingProperties.fromProto(proto.getTradeproperties()!);
      } else if (proto.hasBasicproperties()) {
        return ItemDetailProperties.fromProto(proto.getBasicproperties()!);
      } else if (proto.hasAffixproperties()) {
        return ItemAffixProperties.fromProto(proto.getAffixproperties()!);
      } else {
        throw new Error('Unknown property type');
      }
    };
    return new ItemProperties(
      proto.getAnalyzerid(),
      getProps(),
    );
  }
}
