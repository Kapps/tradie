import { AffixRange as ProtoAffixRange } from "../../protos/Models/Analyzer/Proto/AffixRange_pb";

export enum AffixRangeEntityKind {
  Unknown = 0,
  Modifier = 1,
  Unique = 2,
}

export enum ModKindCategory {
  Enchant = 0,
  Implicit = 1,
  Explicit = 2,
}

export class AffixRange {
  /** Unique hash for the modifier. */
  modHash: string;
  /** The minimum value of the affix. */
  minValue?: number;
  /** The maximum value of the affix. */
  maxValue?: number;
  /** The kind of entity that the affix is applied to. */
  entityKind: AffixRangeEntityKind;
  /** The location categrory of the modifier. */
  modKindCategory: ModKindCategory;

  constructor(modHash: string, entityKind: AffixRangeEntityKind, modKindCategory: ModKindCategory, minValue?: number, maxValue?: number) {
    this.modHash = modHash;
    this.minValue = minValue;
    this.maxValue = maxValue;
    this.entityKind = entityKind;
    this.modKindCategory = modKindCategory;
  }

  toProto(): ProtoAffixRange {
    const proto = new ProtoAffixRange();
    proto.setModhash(this.modHash);
    if (this.minValue !== undefined) {
      proto.setMinvalue(this.minValue);
    }
    if (this.maxValue !== undefined) {
      proto.setMaxvalue(this.maxValue);
    }
    proto.setEntitykind(this.entityKind);
    proto.setModcategory(this.modKindCategory);
    return proto;
  }

  static fromProto(proto: ProtoAffixRange): AffixRange {
    return new AffixRange(
      proto.getModhash(),
      proto.getEntitykind(),
      proto.getModcategory(),
      proto.getMinvalue(),
      proto.getMaxvalue(),
    );
  }
}
