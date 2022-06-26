import { Card, Container, Stack, Text, Title } from '@mantine/core';
import { useAppSelector } from '../../app/hooks';
import { Item } from '../item/item';
import { AnalyzerId, ItemDetailProperties, ItemFlags, ItemTypeProperties } from '../item/itemProperties';
import { SearchResultCard } from './SearchResultCard';
import { selectSearchResults } from './searchSlice';

export function SearchResultList() {
  const searchResults = useAppSelector(selectSearchResults);

  return (
    <Stack align="stretch" justify="center" spacing={0} >
      <Title style={{paddingTop: '10px', paddingBottom: '10px', fontSize: '16px', color: '#888'}} align="center">Displaying {searchResults.length} results</Title>
      {searchResults.map((item, i) => (
        <SearchResultCard index={i} item={item} key={item.rawId} />
      ))}
    </Stack>
  );
}
