import { Card, Container, Stack, Text, Title } from '@mantine/core';
import { useEffect, useRef } from 'react';
import { useAppSelector } from '../../app/hooks';
import { Item } from '../item/item';
import { AnalyzerId, ItemDetailProperties, ItemFlags, ItemTypeProperties } from '../item/itemProperties';
import { SearchResultCard } from './SearchResultCard';
import { selectSearchResults } from './searchSlice';

export function SearchResultList() {
  const searchResults = useAppSelector(selectSearchResults);
  const stackRef = useRef<HTMLDivElement>(null);
  useEffect(() => {
    if (stackRef.current) {
      stackRef.current.scrollIntoView();
    }
  }, [searchResults]);

  return (
    <Stack align="stretch" justify="center" spacing={0} ref={stackRef}>
      <Title style={{paddingTop: '10px', paddingBottom: '10px', fontSize: '16px', color: '#888'}} align="center">Displaying {searchResults.length} results</Title>
      {searchResults.map((item, i) => (
        <SearchResultCard index={i} item={item} key={item.rawId} />
      ))}
    </Stack>
  );
}
