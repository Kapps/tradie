import { Divider, Stack } from '@mantine/core';
import { useSelector } from 'react-redux';
import { substituteValuesInText } from '../../utils/textFormatters';
import { Affix } from '../item/itemProperties';
import { selectModifier } from '../modifiers/modifiersSlice';
import { ModifierLine, PropertyDisplay } from './ModifierLine';
import { ModKind } from './search';

export interface ModifierSectionProps {
  modifiers: Affix[];
  section: ModKind;
  includeSeparator?: boolean;
}

const getDisplayForSection = (section: ModKind) => {
  switch (section) {
    case ModKind.Implicit:
      return PropertyDisplay.Modifier;
    case ModKind.Enchant:
      return PropertyDisplay.Enchant;
    case ModKind.Fractured:
      return PropertyDisplay.Fractured;
    case ModKind.Cosmetic:
      return PropertyDisplay.Enchant;
    case ModKind.Scourge:
      return PropertyDisplay.Corrupted;
    case ModKind.Unknown:
      return PropertyDisplay.Rare;
    case ModKind.Utility:
      return PropertyDisplay.Property;
    case ModKind.Veiled:
      return PropertyDisplay.Craft;
    case ModKind.Explicit:
      return PropertyDisplay.Modifier;
    default:
      console.warn(`Unknown section ${section}`);
      return PropertyDisplay.Value;
  }
};

export function ModifierSection({ modifiers, section, includeSeparator }: ModifierSectionProps) {
  if (includeSeparator === undefined) {
    includeSeparator = true;
  }

  const local = modifiers
    .filter((c) => c.modifier.location === section)
    .map((c) => ({ affix: c, details: useSelector(selectModifier(c.modifier.modifier)) }));

  if (local.length === 0) {
    return <></>;
  }

  return (
    <Stack spacing={0}>
      {local.map((mod, i) => (
        <ModifierLine
          key={i}
          parts={[
            {
              text: substituteValuesInText(mod.details?.text ?? '???', mod.affix.value, mod.affix.value),
              display: getDisplayForSection(section),
            },
          ]}
        />
      ))}
      {includeSeparator && <Divider size={0.05} />}
    </Stack>
  );
}
