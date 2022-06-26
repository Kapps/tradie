import { RangeSlider, TextInput } from '@mantine/core';
import { useEffect, useRef } from 'react';
import { useState } from 'react';
import { extractRange, getRangeDescription } from '../utils/textFormatters';

import styles from './InputSliderRange.module.css';

export interface SliderRangeProps {
  min: number;
  max: number;
  onChange: (val: SliderRangeValue) => void;
  initialValue: [number, number];
  placeholder: string;
}

export type SliderRangeValue = {
  min?: number;
  max?: number;
};

const cleanRange = (range: SliderRangeValue, min?: number, max?: number) => {
  let newMax = range.max === max ? undefined : range.max;
  if (newMax !== undefined && max !== undefined && newMax > max) {
    newMax = max;
  }
  let newMin = range.min === min ? undefined : range.min;
  if (newMin !== undefined && min !== undefined && newMin < min) {
    newMin = min;
  }
  if (max !== undefined) {
    if (newMax !== undefined && newMax > max) {
      newMax = max;
    }
    if (newMin !== undefined && newMin > max) {
      newMin = max;
    }
  }
  if (min !== undefined) {
    if (newMax !== undefined && newMax < min) {
      newMax = max;
    }
    if (newMin !== undefined && newMin < min) {
      newMin = min;
    }
  }
  const newVal = { min: newMin, max: newMax };
  return newVal;
};

export function InputSliderRange({ min, max, onChange, initialValue, placeholder }: SliderRangeProps) {
  const [value, setValue] = useState<SliderRangeValue>(
    cleanRange({ min: initialValue[0], max: initialValue[1] }, min, max),
  );
  const textRef = useRef<HTMLInputElement>(null);
  const [textValue, setTextValue] = useState(getRangeDescription(value.min, value.max, ''));
  const [isValidTextValue, setIsValidTextValue] = useState(true);

  useEffect(() => {
    // console.log('focusing');
    textRef.current?.focus();
  }, []);

  const onValueChanged = (val: SliderRangeValue) => {
    const cleaned = cleanRange(val, min, max);
    if (cleaned.min || cleaned.max) {
      // Don't set the text value in cases like where the min is 1 and you're trying to type 10.
      setTextValue(getRangeDescription(cleaned.min, cleaned.max, ''));
    }
    setIsValidTextValue(true);
    setValue(cleaned);
    onChange(cleaned);
    // console.log('changed', cleaned);
  };

  const onTextChanged = (val: string) => {
    const extracted = extractRange(val);
    // console.log(extracted);
    if (extracted.success) {
      const cleaned = cleanRange({ min: extracted.min, max: extracted.max }, min, max);
      onValueChanged(cleaned);
      if (cleaned.min === undefined && cleaned.max === undefined) {
        setIsValidTextValue(false);
        setTextValue(val);
      }
    } else {
      setTextValue(val);
      setIsValidTextValue(false);
    }
  };

  return (
    <div>
      <TextInput
        // type="number"
        className={`${styles.rangeInput} ${isValidTextValue ? '' : styles.invalid}`}
        autoFocus
        size="xs"
        variant="unstyled"
        placeholder={placeholder}
        ref={textRef}
        onChange={(ev) => onTextChanged(ev.target.value)}
        // defaultValue={getRangeDescription(value.min, value.max)}

        value={textValue}
      />
      <RangeSlider
        labelTransitionDuration={0}
        min={min}
        max={max}
        // step={1}
        value={[value.min ?? min, value.max ?? max]}
        minRange={0}
        label={null}
        //label={(val) => <TextInput value={val} />}
        // labelAlwaysOn
        onChange={([newMin, newMax]) => onValueChanged({ min: newMin, max: newMax })}
      />
    </div>
  );
}
