import { Criteria as ProtoCriteria, ModifierCriteria as ProtoModifierCriteria } from '../../protos/Models/Indexer/Proto/Criteria_pb';

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
  modifier?: CriteriaModifier;
  league?: string;
  category?: string;
  subcategory?: string;

  constructor(id: string, name: string, kind: CriteriaKind, modifier?: CriteriaModifier, league?: string, category?: string, subcategory?: string) {
    this.id = id;
    this.name = name;
    this.kind = kind;
    this.modifier = modifier;
    this.league = league;
    this.category = category;
    this.subcategory = subcategory;
  }

  static fromProto(proto: ProtoCriteria): Criteria {
    return new Criteria(
      proto.getId(),
      proto.getName(),
      proto.getKind(),
      proto.getModifier() ? CriteriaModifier.fromProto(proto.getModifier()!) : undefined,
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
    if (this.modifier) {
      proto.setModifier(this.modifier.toProto());
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

export const getLabelForCriteriaKind = (criteria: Criteria): string => {
  switch (criteria.kind) {
    case CriteriaKind.MODIFIER:
      return criteria.modifier?.kind === ModifierKind.Pseudo ? 'Pseudo' : 'Modifier';
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
  throw new Error(`Unsupported criteria kind ${criteria.kind}`);
};

export enum ModifierKind {
  Standard = 0,
  Pseudo = 1,
}

export class CriteriaModifier {
  hash: string;
  kind: ModifierKind;

  constructor(hash: string, kind: ModifierKind) {
    this.hash = hash;
    this.kind = kind;
  }

  static fromProto(proto: ProtoModifierCriteria): CriteriaModifier {
    return new CriteriaModifier(
      proto.getModifierhash(),
      proto.getKind(),
    );
  }

  toProto(): ProtoModifierCriteria {
    const modifier = new ProtoModifierCriteria();
    modifier.setModifierhash(this.hash);
    modifier.setKind(this.kind);
    return modifier;
  }
}
