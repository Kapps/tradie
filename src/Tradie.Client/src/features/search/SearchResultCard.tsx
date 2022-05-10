import { Card, Center, Container, Space, Stack, Text } from '@mantine/core';
import { useSelector } from 'react-redux';
import { Item } from '../item/item';
import { AnalyzerId, ItemDetailProperties, ItemFlags, ItemTypeProperties } from '../item/itemProperties';
import { selectItemType } from '../itemTypes/itemTypesSlice';
import { ModifierLine, PropertyDisplay } from './ModifierLine';

export interface SearchResultCardProps {
  item: Item;
  index: number;
}

export function SearchResultCard({ item, index }: SearchResultCardProps) {
  const detailProperties = item.findProperty<ItemDetailProperties>(AnalyzerId.ItemDetails);
  const typeProperties = item.findProperty<ItemTypeProperties>(AnalyzerId.ItemType);

  const itemType = useSelector(selectItemType(typeProperties.itemTypeId))!;

  return (
    <Card className={`searchResultCard ${index % 2 == 0 ? 'even' : 'odd'}`}  style={{ height: '100%', margin: 0 }}>
      <Center>
        <Stack spacing={10}>
          <Stack spacing={2}>
            {(detailProperties.name && <ModifierLine parts={[{ text: detailProperties.name, display: PropertyDisplay.Rare }]} />)}
            <ModifierLine parts={[{ text: itemType.name, display: PropertyDisplay.Value }]} />
          </Stack>
          <Stack spacing={2}>
            {(detailProperties.flags & ItemFlags.Corrupted) != 0 &&
            <ModifierLine parts={[{ text: 'Corrupted', display: PropertyDisplay.Corrupted }]} />}
          </Stack>
        </Stack>
      </Center>
    </Card>
  );
}
