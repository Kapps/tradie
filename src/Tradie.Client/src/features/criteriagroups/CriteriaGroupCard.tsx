import { ActionIcon, Button, Container, Group, Select, Space, useMantineTheme } from '@mantine/core';
import { useAppDispatch } from '../../app/hooks';
import CriteriaList from '../criterialist/CriteriaList';
import { CriteriaGroup, addCriteriaGroup, removeCriteriaGroup } from './criteriaGroupsSlice';
import uuid from '../../utils/uuid';
import { ImBin, ImPlus } from 'react-icons/im';

export interface CriteriaGroupProps {
  group: CriteriaGroup;
}

export function CriteriaGroupCard({ group }: CriteriaGroupProps) {
  const dispatch = useAppDispatch();

  const appendCriteriaGroup = () => {
    const id = uuid();
    const newGroup: CriteriaGroup = {
      id,
    };
    return dispatch(addCriteriaGroup(newGroup));
  };

  const deleteCriteriaGroup = () => {
    return dispatch(removeCriteriaGroup(group.id));
  };

  return (
    <Container size="md" style={{ height: '100%', marginTop: '15px', marginBottom: '15px' }}>
      <CriteriaList group={group} />
      <Space h={20} />
      <Group position="apart">
        <Select data={['And', 'Or', 'Weighted']} value="And" />
        <Group position="right">
          <ActionIcon title="Delete Group" onClick={deleteCriteriaGroup}>
            <ImBin color="red" />
          </ActionIcon>
          <ActionIcon color="indigo" title="Add New Group" onClick={appendCriteriaGroup}>
            <ImPlus />
          </ActionIcon>
        </Group>
      </Group>
    </Container>
  );
}
