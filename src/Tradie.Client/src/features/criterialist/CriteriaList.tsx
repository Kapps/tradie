import { Highlight, MultiSelect, Text } from '@mantine/core';
import { ComponentPropsWithoutRef, forwardRef, useEffect, useRef, useState, useTransition } from 'react';
import { useDispatch } from 'react-redux';
import styles from './CriteriaList.module.css';
import { getRandomDescriptionHint, getRandomPlaceholderHints } from '../criteria/criteriaApi';
import { SelectedCriteria } from './SelectedCriteria';
import { selectAvailableCriteria, selectCriteria } from '../criteria/criteriaSlice';
import { useAppSelector } from '../../app/hooks';
import { CriteriaGroup } from '../criteriagroups/criteriaGroupsSlice';
import { useClickOutside } from '@mantine/hooks';
import { addCriteriaValue, CriteriaValue, removeCriteriaValue, selectCriteriaValues } from './criteriaValueSlice';
import uuid from '../../utils/uuid';
import { Criteria, CriteriaKind, getLabelForCriteriaKind } from '../criteria/criteria';
import { setDefaultLeague } from '../leagues/leaguesSlice';
import { getMatchedCriteria } from './criteriaMatcher';

export interface CriteriaListProps {
  group: CriteriaGroup;
}

const maxSelectedValues = 8;
const maxDropdownValues = 20;

type CriteriaListEntry = ComponentPropsWithoutRef<'div'> &
  Criteria & {
    label: string;
    value: string;
    kindLabel: string;
    criteriaId: string;
  };

export default function CriteriaList({ group }: CriteriaListProps) {
  const [placeholderHints] = useState(getRandomPlaceholderHints(3));
  const [descriptionHint] = useState(getRandomDescriptionHint());
  const [isOpen, setIsOpen] = useState(false);
  const [searchText, setSearchText] = useState('');
  const [isPending, startTransition] = useTransition();

  useEffect(() => {
    console.log('opening');
    setIsOpen(true);
  }, []);

  useClickOutside(() => setIsOpen(false));

  const dispatch = useDispatch();

  const allCriteria = useAppSelector(selectAvailableCriteria);
  const selectedCriteria = useAppSelector(selectCriteriaValues(group.id));
  const selectedValues = selectedCriteria?.map((c) => c.id.toString());
  const availableCriteria = getMatchedCriteria(allCriteria, selectedCriteria, searchText, maxDropdownValues).map(
    (c) => ({
      label: c.name,
      value: c.id.toString(),
      criteriaId: c.id.toString(),
      group: `${getLabelForCriteriaKind(c)}s`.toUpperCase(),
      kindLabel: getLabelForCriteriaKind(c),
    }),
  );

  const updateSearchText = (query: string) => {
    startTransition(() => setSearchText(query));
    //setSearchText(query);
  };

  const updateCriteriaValues = (values: string[]) => {
    const removedCriteria = selectedCriteria?.filter((c) => !values.includes(c.criteriaId));
    for (const removed of removedCriteria!) {
      dispatch(removeCriteriaValue(removed));
    }

    const addedCriteria = values
      .filter((v) => !selectedCriteria?.map((c) => c.id).includes(v))
      .map((c) => availableCriteria.find((d) => d.criteriaId === c)!);
    for (const added of addedCriteria) {
      dispatch(
        addCriteriaValue({
          id: added.criteriaId.toString(),
          criteriaId: added.criteriaId,
          groupId: group.id,
          enabled: true,
        }),
      );
      const criteria = allCriteria.find((c) => c.id === added.criteriaId);
      if (criteria?.kind === CriteriaKind.LEAGUE) {
        dispatch(setDefaultLeague({ id: criteria.id }));
      }
    }
  };

  const EntryItem = forwardRef<HTMLDivElement, CriteriaListEntry>(
    ({ label, value, kindLabel, criteriaId, ...others }, ref) => (
      <div className={styles.entryItemContainer} ref={ref} {...others}>
        <Text className={`${styles.entryItemKind} criteria-${kindLabel.toLowerCase()}`} size="xs">
          {kindLabel}
        </Text>
        <Highlight className={styles.entryItemText} size="xs" highlight={searchText}>
          {label}
        </Highlight>
      </div>
    ),
  );
  EntryItem.displayName = 'EntryItem';

  return (
    <div className={styles.filterBar}>
      <MultiSelect
        autoFocus
        searchable
        clearable
        onSearchChange={updateSearchText}
        data={availableCriteria}
        label="Filter Criteria"
        placeholder={`Try: ${placeholderHints.map((c) => c.hint).join(', ')}`}
        maxSelectedValues={maxSelectedValues}
        limit={maxDropdownValues}
        value={selectedValues}
        onChange={updateCriteriaValues}
        valueComponent={(props) => <SelectedCriteria {...props} initiallyOpen={false} allowPopover={!isOpen} />}
        itemComponent={EntryItem}
        className={styles.filterBar}
        variant="default"
        nothingFound="No results found"
        onDropdownOpen={() => setIsOpen(true)}
        onDropdownClose={() => setIsOpen(false)}
        clearSearchOnChange
        initiallyOpened={isOpen}
        description={`Tip: ${descriptionHint.hint}`}
        sx={{
          input: {
            paddingTop: '8px',
            paddingBottom: '8px',
          },
        }}
      />
    </div>
  );
}
