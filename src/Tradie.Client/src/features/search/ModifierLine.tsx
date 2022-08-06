import { Text } from '@mantine/core';
import { ItemRarity } from '../item/itemProperties';

import styles from './ModifierLine.module.css';

export enum PropertyDisplay {
  Unknown,
  Modified,
  Fire,
  Cold,
  Lightning,
  Chaos,
  Property,
  Value,
  Enchant,
  Craft,
  Normal,
  Magic,
  Rare,
  Unique,
  Relic,
  Currency,
  Corrupted,
  Note,
  Name,
  Modifier,
  Fractured,
  Gem,
  Pseudo,
}

export type ModifierPart = {
  text: string;
  display: PropertyDisplay;
};

export interface ModifierLineProps {
  parts: ModifierPart[];
}

export const getPropertyDisplayForRarity = (rarity: ItemRarity) => {
  switch (rarity) {
    case ItemRarity.Normal:
      return PropertyDisplay.Normal;
    case ItemRarity.Magic:
      return PropertyDisplay.Magic;
    case ItemRarity.Rare:
      return PropertyDisplay.Rare;
    case ItemRarity.Unique:
      return PropertyDisplay.Unique;
    case ItemRarity.Gem:
      return PropertyDisplay.Normal;
    case ItemRarity.Relic:
      return PropertyDisplay.Relic;
    default:
      throw new Error('Out of range: ' + rarity);
  }
};

export const getPropertyDisplayForCategory = (category: string) => {
  switch (category) {
    case 'gems':
      return PropertyDisplay.Gem;
    case 'currency':
      return PropertyDisplay.Currency;
    default:
      return PropertyDisplay.Normal;
  }
};

const getClassForDisplay = (display: PropertyDisplay) => {
  switch (display) {
    case PropertyDisplay.Value:
      return styles.value;
    case PropertyDisplay.Fire:
      return styles.fire;
    case PropertyDisplay.Cold:
      return styles.cold;
    case PropertyDisplay.Lightning:
      return styles.lightning;
    case PropertyDisplay.Chaos:
      return styles.chaos;
    case PropertyDisplay.Enchant:
      return styles.enchant;
    case PropertyDisplay.Modified:
      return styles.modified;
    case PropertyDisplay.Craft:
      return styles.crafted;
    case PropertyDisplay.Normal:
      return styles.normal;
    case PropertyDisplay.Magic:
      return styles.magic;
    case PropertyDisplay.Rare:
      return styles.rare;
    case PropertyDisplay.Unique:
      return styles.unique;
    case PropertyDisplay.Currency:
      return styles.currency;
    case PropertyDisplay.Property:
      return styles.property;
    case PropertyDisplay.Corrupted:
      return styles.corrupted;
    case PropertyDisplay.Note:
      return styles.note;
    case PropertyDisplay.Modifier:
      return styles.modifier;
    case PropertyDisplay.Name:
      return styles.name;
    case PropertyDisplay.Fractured:
      return styles.fractured;
    case PropertyDisplay.Gem:
      return styles.gem;
    case PropertyDisplay.Relic:
      return styles.relic;
    case PropertyDisplay.Pseudo:
      return styles.pseudo;
    default:
      return styles.default;
  }
};

export function ModifierLine({ parts }: ModifierLineProps) {
  return (
    <div>
      <Text align="center">
        {parts.map((part, index) => (
          <span key={index} className={getClassForDisplay(part.display)}>{part.text}</span>
        ))}
      </Text>
    </div>
  );
}
