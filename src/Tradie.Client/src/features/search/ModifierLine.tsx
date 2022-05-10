import { Text } from '@mantine/core';

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
  Currency,
  Corrupted,
}

export type ModifierPart = {
  text: string;
  display: PropertyDisplay;
};

export interface ModifierLineProps {
  parts: ModifierPart[];
}

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
  default:
    return styles.default;
  }
};

export function ModifierLine({ parts }: ModifierLineProps) {
  return (
    <div>
      {parts.map((part, index) => (
        <Text align="center" className={getClassForDisplay(part.display)} key={index}>
          {part.text}
        </Text>
      ))}
    </div>
  );
}
