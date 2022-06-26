import { useTheme } from '@emotion/react';
import { Button, Card, Grid, Group, Space, Tabs, Text } from '@mantine/core';
import { ImFloppyDisk, ImPencil, ImPlus } from 'react-icons/im';
import useDarkMode from 'use-dark-mode';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { selectCriteria } from '../criteria/criteriaSlice';
import { CriteriaGroupCard } from '../criteriagroups/CriteriaGroupCard';
import { clearCriteriaGroups, selectCriteriaGroups } from '../criteriagroups/criteriaGroupsSlice';
import { selectCriteriaValues } from '../criterialist/criteriaValueSlice';
import { AffixRange, ModKey, ModKind, SearchGroup, SearchQuery, SortKind, SortOrder } from '../search/search';
import { search } from '../search/searchApi';
import { performSearch } from '../search/searchSlice';

export function FilterPanel() {
  const darkMode = useDarkMode();
  const theme = useTheme();

  const dispatch = useAppDispatch();
  const criteriaGroups = useAppSelector(selectCriteriaGroups);

  const clearFilterGroups = () => {
    dispatch(clearCriteriaGroups());
  };

  const doSearch = () => {
    dispatch(performSearch());
  };

  return (
    <Tabs
      variant="pills"
      className="tabBar"
      tabPadding={0}
      style={{ position: 'sticky', top: '10px', marginTop: 'auto', marginBottom: 'auto', zIndex: 1 }}
      styles={(theme) => ({
        tabsListWrapper: {
          backgroundColor: darkMode.value ? theme.colors.dark[8] : theme.colors.gray[2],
        },
      })}
    >
      <Tabs.Tab label="Default" icon={<ImPencil />} active>
        {criteriaGroups.map((group, i) => (
          <Card key={group.id} className={`criteriaGroup ${i % 2 == 0 ? 'even' : 'odd'}`}>
            <CriteriaGroupCard group={group} />
          </Card>
        ))}
        <Space h={5} />
        <Grid>
          <Grid.Col span={6}>
            <Group position="left">
              <Button onClick={clearFilterGroups} color="gray">
                Reset
              </Button>
            </Group>
          </Grid.Col>
          <Grid.Col span={6}>
            <Group position="right">
              <Button onClick={doSearch} color="primary" style={{ width: '200px' }}>
                Search
              </Button>
            </Group>
          </Grid.Col>
        </Grid>
      </Tabs.Tab>
      <Tabs.Tab label="Jewels" icon={<ImFloppyDisk />}>
        <Card>
          <Text>Arrr</Text>
        </Card>
      </Tabs.Tab>
      <Tabs.Tab label="Other Saved" icon={<ImFloppyDisk />}>
        <Card>
          <Text>Arrr</Text>
        </Card>
      </Tabs.Tab>
      <Tabs.Tab icon={<ImPlus />} color="red">
        <Card>
          <Text>Arrr</Text>
        </Card>
      </Tabs.Tab>
    </Tabs>
  );
}
