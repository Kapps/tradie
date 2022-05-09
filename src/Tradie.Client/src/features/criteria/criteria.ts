import { League } from "../leagues/league";
import { Modifier } from "../item/modifier";

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
