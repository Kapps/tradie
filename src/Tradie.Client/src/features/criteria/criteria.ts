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

export type Criteria = {
  id: string;
  name: string;
  kind: CriteriaKind;
  modifier?: Modifier;
  league?: League;
  category?: string;
  subcategory?: string;
};

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
