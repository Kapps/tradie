import { RangeSlider, TextInput } from '@mantine/core';
import { useState } from 'react';
import { getRangeDescription } from '../utils/textFormatters';

export interface SliderRangeProps {
  min: number;
  max: number;
  onChange: (val: SliderRangeValue) => void;
  initialValue: [number, number];
}

export type SliderRangeValue = {
  min?: number;
  max?: number;
};

export function InputSliderRange({ min, max, onChange, initialValue }: SliderRangeProps) {
  const [value, setValue] = useState<SliderRangeValue>({ min: initialValue[0], max: initialValue[1] });

  const onValueChanged = (val: SliderRangeValue) => {
    setValue(val);
    onChange(val);
  };

  return (
    <div>
      <TextInput value={getRangeDescription(value.min, value.max)} />
      <RangeSlider
        labelTransitionDuration={0}
        min={min}
        max={max}
        value={[value.min ?? min, value.max ?? max]}
        minRange={0}
        label={null}
        //label={(val) => <TextInput value={val} />}
        onChange={([newMin, newMax]) => onValueChanged({ min: newMin, max: newMax })}
      />
    </div>
  );
}
