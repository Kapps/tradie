import {
  AffixRange as ProtoAffixRange,
  SearchGroup as ProtoSearchGroup,
  SearchQuery as ProtoSearchQuery,
  SortOrder as ProtoSortOrder,
} from "../../protos/Models/Indexer/Proto/SearchQuery_pb";

import {
  ModKey as ProtoModKey,
} from "../../protos/Models/Analyzer/Proto/ModifierTypes_pb";

export enum SortKind {
  Price = 1,
  Modifier = 2
}

export class SortOrder {
  kind: SortKind;
  modifier?: ModKey;

  constructor(kind: SortKind, modifier?: ModKey) {
    this.kind = kind;
    this.modifier = modifier;
  }

  toProto(): ProtoSortOrder {
    const proto = new ProtoSortOrder();
    proto.setSortkind(this.kind);
    if (this.modifier) {
      const protoModKey = new ProtoModKey();
      protoModKey.setLocation(this.modifier.location);
      protoModKey.setModifier(this.modifier.modifier);
      proto.setModifier(protoModKey);
    }
    return proto;
  }

  static fromProto(proto: ProtoSortOrder): SortOrder {
    return new SortOrder(
      proto.getSortkind(),
      proto.hasModifier() ? ModKey.fromProto(proto.getModifier()!) : undefined,
    );
  }
}

export enum ModKind {
  Unknown = 0,
  Implicit = 1,
  Explicit = 2,
  Enchant = 3,
  Scourge = 4,
  Utility = 5,
  Fractured = 6,
  Cosmetic = 7,
  Veiled = 8,
  Crafted = 9,
  Total = 10,
  Pseudo = 11
}

export enum SearchGroupKind {
  And = 1,
  Sum = 2,
}

export class ModKey {
  modifier: string;
  location: ModKind;

  constructor(modifier: string, location: ModKind) {
    this.modifier = modifier;
    this.location = location;
  }

  toProto(): ProtoModKey {
    const proto = new ProtoModKey();
    proto.setModifier(this.modifier);
    proto.setLocation(this.location);
    return proto;
  }

  static fromProto(proto: ProtoModKey): ModKey {
    return new ModKey(proto.getModifier(), <ModKind>proto.getLocation());
  }
}

export class AffixRange {
  modifier: ModKey;
  minValue?: number;
  maxValue?: number;

  constructor(modifier: ModKey, minValue?: number, maxValue?: number) {
    this.modifier = modifier;
    this.minValue = minValue;
    this.maxValue = maxValue;
  }

  toProto(): ProtoAffixRange {
    const proto = new ProtoAffixRange();
    proto.setKey(this.modifier.toProto());
    proto.setMinvalue(this.minValue ?? -Infinity);
    proto.setMaxvalue(this.maxValue ?? Infinity);
    return proto;
  }

  static fromProto(proto: ProtoAffixRange): AffixRange {
    return new AffixRange(
      ModKey.fromProto(proto.getKey()!),
      proto.getMinvalue(),
      proto.getMaxvalue(),
    );
  }
}

export class SearchGroup {
  kind: SearchGroupKind;
  ranges: AffixRange[];

  constructor(kind: SearchGroupKind, ranges: AffixRange[]) {
    this.kind = kind;
    this.ranges = ranges;
  }

  toProto(): ProtoSearchGroup {
    const proto = new ProtoSearchGroup();
    proto.setGroupkind(this.kind);
    this.ranges.forEach(range => proto.addRanges(range.toProto()));
    return proto;
  }

  static fromProto(proto: ProtoSearchGroup): SearchGroup {
    return new SearchGroup(
      <SearchGroupKind>proto.getGroupkind(),
      proto.getRangesList().map(r => AffixRange.fromProto(r))
    );
  }
}

export class SearchQuery {
  groups: SearchGroup[];
  sort: SortOrder;

  constructor(groups: SearchGroup[], sort: SortOrder) {
    this.groups = groups;
    this.sort = sort;
  }

  toProto(): ProtoSearchQuery {
    const proto = new ProtoSearchQuery();
    this.groups.forEach(g => proto.addGroups(g.toProto()));
    proto.setSort(this.sort.toProto());
    return proto;
  }

  static fromProto(proto: ProtoSearchQuery): SearchQuery {
    return new SearchQuery(
      proto.getGroupsList().map(c => SearchGroup.fromProto(c)),
      proto.hasSort() ? SortOrder.fromProto(proto.getSort()!) : new SortOrder(SortKind.Price),
    );
  }
}
