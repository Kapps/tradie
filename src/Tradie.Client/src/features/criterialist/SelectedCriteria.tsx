import {
  ActionIcon,
  Badge,
  Card,
  Center,
  Checkbox,
  Divider,
  Group,
  MultiSelectValueProps,
  Popover,
  RangeSlider,
  Space,
  Switch,
  Text,
} from '@mantine/core';
import { useClickOutside } from '@mantine/hooks';
import { FormEventHandler, MouseEventHandler, useRef } from 'react';
import { useState } from 'react';
import { ImCheckboxChecked, ImCheckmark, ImCross, ImFloppyDisk } from 'react-icons/im';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { InputSliderRange, SliderRangeValue } from '../../common/InputSliderRange';
import { getRangeDescription, substituteValuesInText } from '../../utils/textFormatters';
import { AffixRangeEntityKind, ModKindCategory } from '../affixRanges/affixRange';
import { selectAffixRange } from '../affixRanges/affixRangesSlice';
import { CriteriaKind } from '../criteria/criteria';
import { selectCriteria } from '../criteria/criteriaSlice';

import styles from './CriteriaList.module.css';
import { clearCriteriaValues, CriteriaValue, selectCriteriaValue, updateCriteriaValue } from './criteriaValueSlice';

const getMinMax = (criteriaId: string) => {
  const criteria = useAppSelector(selectCriteria(criteriaId))!;
  if (criteria.kind === CriteriaKind.MODIFIER) {
    const range = useAppSelector(
      selectAffixRange(criteria.modifierHash!, AffixRangeEntityKind.Modifier, ModKindCategory.Explicit),
    );
    return range ? [range?.minValue, range?.maxValue] : null;
  }
  return null;
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
  const helpText = getRangeDescription(minValue, maxValue);
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
  initiallyOpen,
  ...others
}: MultiSelectValueProps & {
  value: string;
  allowPopover: boolean;
  criteriaId: string;
  kindLabel: string;
  initiallyOpen: boolean;
}) {
  const criteriaValue = useAppSelector(selectCriteriaValue(criteriaId))!;
  const [min, max] = getMinMax(criteriaId) ?? [0, 0];
  const [val, setVal] = useState<[number, number]>([
    criteriaValue.minValue ?? min ?? -Infinity,
    criteriaValue.maxValue ?? max ?? Infinity,
  ]);
  const [opened, setOpened] = useState(initiallyOpen);

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

  const onValueChanged = (val: SliderRangeValue) => {
    setVal([val.min ?? min ?? 0, val.max ?? max ?? 0]);
  };

  const onEnabledChanged: FormEventHandler<HTMLInputElement> = (e) => {
    dispatch(updateCriteriaValue({ ...criteriaValue, enabled: e.currentTarget.checked }));
  };

  const onClose = () => {
    const [minVal, maxVal] = val;
    dispatch(
      updateCriteriaValue({
        ...criteriaValue,
        minValue: minVal === min ? undefined : minVal,
        maxValue: maxVal === max ? undefined : maxVal,
      }),
    );
    setOpened(false);
  };

  const onCardKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) => {
    if (e.key === 'Escape' || e.key === 'Enter') {
      onClose();
    }
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
        arrowSize={12}
        radius="md"
        closeOnClickOutside={true}
        // withCloseButton
        // closeOnClickOutside={false}
        // onMouseEnter={() => setOpened(true)}
        // onMouseLeave={() => setOpened(false)}
        // trapFocus={false}
        target={
          //<Badge<typeof ValueTextComponent>
          <Badge
            className={`${styles.modBadge} ${criteriaValue.enabled ? '' : styles.disabled}`}
            //variant={criteriaValue.enabled ? 'dot' : 'light'}
            // className={`${styles.modBadge}`}
            variant="dot"
            color={criteriaValue.enabled ? undefined : 'gray'}
            size="lg"
            radius={0}
            rightSection={
              <ActionIcon size="sm" color="blue" variant="transparent" onMouseDown={onRemoveClick}>
                <ImCross size={7} style={{ textAlign: 'right' }} />
              </ActionIcon>
            }
          >
            {substituteValuesInText(label, criteriaValue.minValue, criteriaValue.maxValue)}
          </Badge>
        }
        width={400}
        position="top"
        withArrow
      >
        <Card onKeyDown={onCardKeyDown} style={{ padding: '0', margin: '0' }}>
          <Center>
            <Text size="md" weight="bolder">
              {label}
            </Text>
          </Center>
          <Space h={20} />
          {/*<RangeSlider
            min={min}
            max={max}
            minRange={0}
            labelAlwaysOn
            //onBlur={onClose}
            //value={[criteria.minValue ?? 0, criteria.maxValue ?? Infinity]}
            value={val}
            onChange={setVal}
          />*/}
          {min && max ? (
            <InputSliderRange
              min={min ?? 0}
              max={max ?? 0}
              placeholder="Examples: 46, >=46, 20-50, <=70"
              initialValue={[criteriaValue.minValue ?? min ?? 0, criteriaValue.maxValue ?? max ?? 0]}
              //onBlur={onClose}
              //value={[criteria.minValue ?? 0, criteria.maxValue ?? Infinity]}
              //value={val}
              onChange={onValueChanged}
            />
          ) : (
            ''
          )}
          <Space h={20} />
          <Divider />
          <Space h={10} />
          <Group position="apart">
            <Switch
              color="orange"
              size="lg"
              offLabel="OFF"
              onLabel="ON"
              checked={criteriaValue.enabled}
              onChange={onEnabledChanged}
            />
            <ActionIcon color="orange" title="Save" onClick={onClose}>
              <ImCheckmark />
            </ActionIcon>
          </Group>
        </Card>
      </Popover>
    </div>
  );
}
