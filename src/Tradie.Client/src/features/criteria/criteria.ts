import { Criteria as ProtoCriteria } from '../../protos/Models/Indexer/Proto/Criteria_pb';
import { League } from '../leagues/league';
import { Modifier } from '../modifiers/modifier';

export enum CriteriaKind {
  UNKNOWN = 0,
  MODIFIER = 1,
  LEAGUE = 2,
  CATEGORY = 3,
  SUBCATEGORY = 4,
  ITEM_TYPE = 5,
  UNIQUE = 6,
}

export class Criteria {
  id: string;
  name: string;
  kind: CriteriaKind;
  modifierHash?: string;
  league?: string;
  category?: string;
  subcategory?: string;

  constructor(id: string, name: string, kind: CriteriaKind, modifierHash: string, league?: string, category?: string, subcategory?: string) {
    this.id = id;
    this.name = name;
    this.kind = kind;
    this.modifierHash = modifierHash;
    this.league = league;
    this.category = category;
    this.subcategory = subcategory;
  }

  static fromProto(proto: ProtoCriteria): Criteria {
    return new Criteria(
      proto.getId(),
      proto.getName(),
      proto.getKind(),
      proto.getModifierhash(),
      proto.getLeague(),
      proto.getCategory(),
      proto.getSubcategory(),
    );
  }

  toProto(): ProtoCriteria {
    const proto = new ProtoCriteria();
    proto.setId(this.id);
    proto.setName(this.name);
    proto.setKind(this.kind);
    if (this.modifierHash) {
      proto.setModifierhash(this.modifierHash);
    }
    if (this.league) {
      proto.setLeague(this.league);
    }
    if (this.category) {
      proto.setCategory(this.category);
    }
    if (this.subcategory) {
      proto.setSubcategory(this.subcategory);
    }
    return proto;
  }
}

export const getLabelForCriteriaKind = (kind: CriteriaKind): string => {
  switch (kind) {
    case CriteriaKind.MODIFIER:
      return 'Modifier';
    case CriteriaKind.LEAGUE:
      return 'League';
    case CriteriaKind.CATEGORY:
      return 'Category';
    case CriteriaKind.SUBCATEGORY:
      return 'Subcategory';
    case CriteriaKind.ITEM_TYPE:
      return 'Item Type';
    case CriteriaKind.UNIQUE:
      return 'Unique';
  }
  throw new Error(`Unsupported criteria kind ${kind}`);
};
