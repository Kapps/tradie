import { ItemType as ProtoItemType } from "../../protos/Models/Analyzer/Proto/ItemType_pb";
import { Requirements as ProtoRequirements } from "../../protos/Models/Analyzer/Proto/Requirements_pb";

export class Requirements {
  dex: number;
  str: number;
  int: number;
  level: number;

  constructor(dex: number, str: number, int: number, level: number) {
    this.dex = dex;
    this.str = str;
    this.int = int;
    this.level = level;
  }

  toProto(): ProtoRequirements {
    const proto = new ProtoRequirements();
    proto.setDex(this.dex);
    proto.setStr(this.str);
    proto.setInt(this.int);
    proto.setLevel(this.level);
    return proto;
  }

  static fromProto(proto: ProtoRequirements): Requirements {
    return new Requirements(
      proto.getDex(),
      proto.getStr(),
      proto.getInt(),
      proto.getLevel()
    );
  }
}

export class ItemType {
  id: number;
  name: string;
  category: string;
  subcategoriesList: Array<string>;
  requirements?: Requirements;
  width: number;
  height: number;
  iconUrl: string;

  constructor(id: number, name: string, category: string, subcategoriesList: Array<string>, width: number, height: number, iconUrl: string, requirements?: Requirements) {
    this.id = id;
    this.name = name;
    this.category = category;
    this.subcategoriesList = subcategoriesList;
    this.requirements = requirements;
    this.width = width;
    this.height = height;
    this.iconUrl = iconUrl;
  }

  toProto(): ProtoItemType {
    const proto = new ProtoItemType();
    proto.setId(this.id);
    proto.setName(this.name);
    proto.setCategory(this.category);
    proto.setSubcategoriesList(this.subcategoriesList);
    proto.setWidth(this.width);
    proto.setHeight(this.height);
    proto.setIconurl(this.iconUrl);

    if (this.requirements) {
      proto.setRequirements(this.requirements.toProto());
    }

    return proto;
  }

  static fromProto(proto: ProtoItemType): ItemType {
    return new ItemType(
      proto.getId(),
      proto.getName(),
      proto.getCategory(),
      proto.getSubcategoriesList(),
      proto.getWidth(),
      proto.getHeight(),
      proto.getIconurl(),
      proto.hasRequirements() ? Requirements.fromProto(proto.getRequirements()!) : undefined,
    );
  }

}
