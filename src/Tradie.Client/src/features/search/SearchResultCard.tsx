import { Card, Center, Container, Divider, Grid, Image, Space, Stack, Text } from '@mantine/core';
import { useSelector } from 'react-redux';
import { substituteValuesInText } from '../criterialist/SelectedCriteria';
import { Item } from '../item/item';
import { AnalyzerId, ItemAffixProperties, ItemDetailProperties, ItemFlags, ItemListingProperties, ItemTypeProperties } from '../item/itemProperties';
import { selectItemType } from '../itemTypes/itemTypesSlice';
import { Modifier } from '../modifiers/modifier';
import { selectAllModifiers } from '../modifiers/modifiersSlice';
import { ModifierLine, PropertyDisplay } from './ModifierLine';
import { ModifierSection } from './ModifierSection';
import { ModKind } from './search';

import styles from './SearchResultCard.module.css';

export interface SearchResultCardProps {
  item: Item;
  index: number;
}

export function SearchResultCard({ item, index }: SearchResultCardProps) {
  const detailProperties = item.findProperty<ItemDetailProperties>(AnalyzerId.ItemDetails);
  const typeProperties = item.findProperty<ItemTypeProperties>(AnalyzerId.ItemType);
  const tradeProperties = item.findProperty<ItemListingProperties>(AnalyzerId.TradeAttributes);
  const affixProperties = item.findProperty<ItemAffixProperties>(AnalyzerId.Modifiers);

  const itemType = useSelector(selectItemType(typeProperties.itemTypeId))!;

  return (
    <Card className={`searchResultCard ${index % 2 == 0 ? 'even' : 'odd'}`} style={{ height: '100%', margin: 0 }}>
      <Grid>
        <Grid.Col span={3}>
          <Center style={{ height: '100%' }}>
            <Image src={itemType.iconUrl} alt={itemType.name} />
          </Center>
        </Grid.Col>
        <Grid.Col span={6}>
          <Center>
            <Stack spacing={5}>
              <Stack spacing={2}>
                {(detailProperties.name && <ModifierLine parts={[{ text: detailProperties.name, display: PropertyDisplay.Rare }]} />)}
                <ModifierLine parts={[{ text: itemType.name, display: PropertyDisplay.Name }]} />
                <Space h={8} />
              </Stack>
              <Stack spacing={2}>
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Enchant} includeSeparator={false} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Utility} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Implicit} includeSeparator={false} />
                <div className={styles.separator} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Scourge} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Fractured} includeSeparator={false} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Explicit} includeSeparator={false} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Veiled} includeSeparator={false} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Cosmetic} />
                <div className={styles.separator} />
              </Stack>
              <Stack spacing={2}>
                {(detailProperties.flags & ItemFlags.Corrupted) != 0 &&
                  <ModifierLine parts={[{ text: 'Corrupted', display: PropertyDisplay.Corrupted }]} />}
                {tradeProperties.note && <ModifierLine parts={[{ text: tradeProperties.note, display: PropertyDisplay.Note }]} />}
              </Stack>
            </Stack>
          </Center>
        </Grid.Col>
        <Grid.Col span={3}>

        </Grid.Col>
      </Grid>
    </Card>
  );
}
