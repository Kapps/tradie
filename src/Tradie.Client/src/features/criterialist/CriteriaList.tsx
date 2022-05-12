import { Highlight, MultiSelect, Text } from '@mantine/core';
import { ComponentPropsWithoutRef, forwardRef, useEffect, useRef, useState, useTransition } from 'react';
import { useDispatch } from 'react-redux';
import styles from './CriteriaList.module.css';
import { getRandomDescriptionHint, getRandomPlaceholderHints } from '../criteria/criteriaApi';
import { SelectedCriteria } from './SelectedCriteria';
import { selectAvailableCriteria } from '../criteria/criteriaSlice';
import { useAppSelector } from '../../app/hooks';
import { CriteriaGroup } from '../criteriagroups/criteriaGroupsSlice';
import { useClickOutside } from '@mantine/hooks';
import { addCriteriaValue, CriteriaValue, removeCriteriaValue, selectCriteriaValues } from './criteriaValueSlice';
import uuid from '../../utils/uuid';
import { Criteria, CriteriaKind, getLabelForCriteriaKind } from '../criteria/criteria';

export interface CriteriaListProps {
  group: CriteriaGroup;
}

const getMatchedCriteria = (criteria: Criteria[], selectedCriteriaValues: CriteriaValue[], searchText: string) => {
  const selectedCriteria = selectedCriteriaValues.map(c => criteria.find(d => d.id === c.criteriaId)!);
  const selectedLookup = new Set<Criteria>(selectedCriteria);
  const searchLowerified = (searchText || '').toLowerCase();
  const results = new Array<Criteria>();

  for (const crit of criteria) {
    if (crit.kind !== CriteriaKind.MODIFIER && selectedCriteria.some(c => c.kind === crit.kind)) {
      continue;
    }
    if (crit.name.toLowerCase().includes(searchLowerified) || selectedLookup.has(crit)) {
      results.push(crit);
      selectedLookup.delete(crit);
    }
    if (results.length > 20) {
      break;
    }
  }

  for (const remaining of selectedLookup) {
    results.push(remaining);
  }


  return results;
};

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
  //const selectedCriteria = useAppSelector(selectCriteriaGroup(group.id));
  const selectedCriteria = useAppSelector(selectCriteriaValues(group.id));
  const selectedValues = selectedCriteria?.map((c) => c.id.toString());
  const availableCriteria = getMatchedCriteria(allCriteria, selectedCriteria, searchText).map((c) => ({
  //const availableCriteria = allCriteria.map((c) => ({
    label: c.name,
    value: c.id.toString(),
    criteriaId: c.id.toString(),
    group: `${getLabelForCriteriaKind(c.kind)}s`.toUpperCase(),
    kindLabel: getLabelForCriteriaKind(c.kind),
    //disableSelection: selectedCriteria.some((d) => c.id.toString() === d.criteriaId),
    //criteria: { criteriaId: c.id.toString(), id: c.id.toString(), groupId: group.id },
  }));

  const updateSearchText = (query: string) => {
    //startTransition(() => setSearchText(query));
    setSearchText(query);
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
        }),
      );
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
        //onSearchChange={(query) => setSearchText(query)}
        onSearchChange={updateSearchText}
        data={availableCriteria}
        label="Filter Criteria"
        placeholder={`Try: ${placeholderHints.map((c) => c.hint).join(', ')}`}
        maxSelectedValues={20}
        limit={20}
        value={selectedValues}
        onChange={updateCriteriaValues}
        valueComponent={(props) => <SelectedCriteria {...props} allowPopover={!isOpen} />}
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
