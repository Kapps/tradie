import { Card, Center, Container, Space, Text } from '@mantine/core';
import { Item } from '../item/item';
import { AnalyzerId, ItemDetailProperties, ItemFlags, ItemTypeProperties } from '../item/itemProperties';
import { ModifierLine, PropertyDisplay } from './ModifierLine';

export interface SearchResultCardProps {
  item: Item;
  index: number;
}

export function SearchResultCard({ item, index }: SearchResultCardProps) {
  const detailProperties = item.findProperty<ItemDetailProperties>(AnalyzerId.ItemDetails);
  const typeProperties = item.findProperty<ItemTypeProperties>(AnalyzerId.ItemType);
  return (
    <Card className={`searchResultCard ${index % 2 == 0 ? 'even' : 'odd'}`}  style={{ height: '100%', margin: 0 }}>
      <Center>
        <ModifierLine parts={[{ text: detailProperties.name, display: PropertyDisplay.Rare }]} />
        {(detailProperties.flags & ItemFlags.Corrupted) != 0 && <Text color="red">Corrupted</Text>}
      </Center>
    </Card>
  );
}
