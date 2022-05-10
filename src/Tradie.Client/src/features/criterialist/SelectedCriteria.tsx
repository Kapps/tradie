import {
  ActionIcon,
  Badge,
  Card,
  Center,
  MultiSelectValueProps,
  Popover,
  RangeSlider,
  Space,
  Text,
} from '@mantine/core';
import { useClickOutside } from '@mantine/hooks';
import { MouseEventHandler, useRef } from 'react';
import { useState } from 'react';
import { ImCross } from 'react-icons/im';
import { useAppDispatch, useAppSelector } from '../../app/hooks';

import styles from './CriteriaList.module.css';
import { clearCriteriaValues, CriteriaValue, selectCriteriaValue, updateCriteriaValue } from './criteriaValueSlice';

const getRangeDescription = (text: string, minValue?: number, maxValue?: number) => {
  if (minValue && maxValue && minValue === maxValue) {
    return `${minValue}`;
  }
  if (minValue && maxValue) {
    return `${minValue}-${maxValue}`;
  }
  if (minValue && !maxValue) {
    return `>${minValue}`;
  }
  if (!minValue && maxValue) {
    return `<${maxValue}`;
  }
  return '#';
};

const substituteValuesInText = (text: string, min?: number, max?: number) => {
  const numValues = text.split('').filter((c) => c === '#').length;
  if (numValues === 0) {
    return text;
  }
  if (numValues === 1) {
    return text.replace(/#/, `${getRangeDescription(text, min, max)}`);
  }
  let usedValues = 0;
  const vals = [min ?? '#', max ?? '#'];
  return text
    .split('')
    .map((c) => (c === '#' ? `${vals[usedValues++]}` : c))
    .join('');
};

export function ValueTextComponent({
  text,
  minValue,
  maxValue,
  ...others
}: {
  text: string;
  minValue?: number;
  maxValue?: number;
}) {
  const helpText = getRangeDescription(text, minValue, maxValue);
  return (
    <div {...others}>
      <Center>
        <Text size="sm">{text}</Text>
      </Center>
      <Space h={5} />
      {helpText && <Text size="xs">{helpText}</Text>}
    </div>
  );
}

export function SelectedCriteria({
  value,
  label,
  onRemove,
  classNames,
  allowPopover,
  criteriaId,
  kindLabel,
  ...others
}: MultiSelectValueProps & { value: string; allowPopover: boolean; criteriaId: string; kindLabel: string }) {
  const criteria = useAppSelector(selectCriteriaValue(criteriaId))!;
  const [val, setVal] = useState<[number, number]>([criteria.minValue ?? -Infinity, criteria.maxValue ?? Infinity]);
  const [opened, setOpened] = useState(false);

  const dispatch = useAppDispatch();

  useClickOutside(() => setOpened(false));

  const onClick: MouseEventHandler<HTMLDivElement> = (e) => {
    setOpened(true);
    e.stopPropagation();
    e.preventDefault();
  };

  const onRemoveClick: MouseEventHandler<HTMLButtonElement> = (e) => {
    // It actually takes in an event handler, but the types don't recognize that.
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (onRemove as any)(e);
    e.stopPropagation();
    e.preventDefault();
  };

  const onClose = () => {
    const [min, max] = val;
    dispatch(
      updateCriteriaValue({
        ...criteria,
        minValue: min === -Infinity ? undefined : min,
        maxValue: max === Infinity ? undefined : max,
      }),
    );
    setOpened(false);
  };

  /*const onCriteriaValueChanged = (value: [number, number]) => {
    const [min, max] = value;
    dispatch(
      updateCriteriaValue({
        ...criteria,
        minValue: min === 0 ? undefined : min,
        maxValue: max === 100 ? undefined : max,
      }),
    );
  };*/

  return (
    <div {...others} className={styles.modDetails}>
      <Popover
        opened={opened}
        transitionDuration={0}
        onClose={onClose}
        onMouseDown={onClick}
        // closeO nClickOutside={false}
        // onMouseEnter={() => setOpened(true)}
        // onMouseLeave={() => setOpened(false)}
        // trapFocus={false}
        target={
          //<Badge<typeof ValueTextComponent>
          <Badge
            className={styles.modBadge}
            variant="dot"
            /*minValue={criteria.minValue}
            maxValue={criteria.maxValue}
            text={label}
            component={ValueTextComponent}*/
            /*color="gray"*/
            sx={{ textTransform: 'unset', fontWeight: 700, backgroundColor: '#0a0a16' }}
            size="lg"
            radius={0}
            rightSection={
              <ActionIcon size="sm" color="blue" variant="transparent" onMouseDown={onRemoveClick}>
                <ImCross size={7} style={{ textAlign: 'right' }} />
              </ActionIcon>
            }
          >
            {substituteValuesInText(label, criteria.minValue, criteria.maxValue)}
          </Badge>
        }
        width={400}
        position="top"
        withArrow
      >
        <Card>
          <Text>
            {label} - {value}
          </Text>
          <Space h={20} />
          <RangeSlider
            //onBlur={onClose}
            //value={[criteria.minValue ?? 0, criteria.maxValue ?? Infinity]}
            value={val}
            onChange={setVal}
          />
        </Card>
      </Popover>
    </div>
  );
}
