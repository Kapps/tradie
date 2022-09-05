import { Card, Container, Stack, Text, Title } from '@mantine/core';
import { useEffect, useRef } from 'react';
import { useAppSelector } from '../../app/hooks';
import { Item } from '../item/item';
import { AnalyzerId, ItemDetailProperties, ItemFlags, ItemTypeProperties } from '../item/itemProperties';
import { SearchResultCard } from './SearchResultCard';
import { selectSearchResults } from './searchSlice';

import styles from './SearchResultList.module.css';

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
      <Title className={styles.searchResultTitle} align="center">
        Displaying {searchResults.length} results
      </Title>
      {searchResults.map((entry, i) => (
        <SearchResultCard index={i} entry={entry} key={entry.item.rawId} />
      ))}
    </Stack>
  );
}
