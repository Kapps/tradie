import { Item as ProtoItem } from '../../protos/Models/Analyzer/Proto/Item_pb';
import { AnalyzerId, ItemAffixProperties, ItemDetailProperties, ItemListingProperties, ItemProperties, ItemTypeProperties } from "./itemProperties";

export class Item {
  rawId: string;
  properties: ItemProperties[];

  constructor(rawId: string, properties: ItemProperties[]) {
    this.rawId = rawId;
    this.properties = properties;
  }

  toProto(): ProtoItem {
    const proto = new ProtoItem();
    proto.setRawid(this.rawId);
    for (const property of this.properties) {
      proto.addProperties(property.toProto());
    }
    return proto;
  }

  static fromProto(proto: ProtoItem): Item {
    return new Item(
      proto.getRawid(),
      proto.getPropertiesList().map(property => ItemProperties.fromProto(property)),
    );
  }

  findProperty<T extends ItemDetailProperties | ItemAffixProperties | ItemTypeProperties | ItemListingProperties>(id: AnalyzerId): T {
    const res = this.properties.find(property => property.analyzerId === id)?.property as T;
    if (!res) {
      throw new Error(`Failed to find required property ${id}`);
    }
    return res;
  }
}
