/**
 * Returns a human readable description of the two numbers as a range.
 */
export const getRangeDescription = (minValue?: number, maxValue?: number) => {
  if (minValue && maxValue && minValue === maxValue) {
    return `${minValue}`;
  }
  if (minValue && maxValue) {
    return `${minValue}-${maxValue}`;
  }
  if (minValue && !maxValue) {
    return `>=${minValue}`;
  }
  if (!minValue && maxValue) {
    return `<=${maxValue}`;
  }
  return '#';
};

/**
 * Given a value-independent string, replaces the numbers within the text with the given range numbers.
 */
export const substituteValuesInText = (text: string, min?: number, max?: number) => {
  const numValues = text.split('').filter((c) => c === '#').length;
  if (numValues === 0) {
    return text;
  }
  if (numValues === 1) {
    return text.replace(/#/, `${getRangeDescription(min, max)}`);
  }
  let usedValues = 0;
  const vals = [min ?? '#', max ?? '#'];
  return text
    .split('')
    .map((c) => (c === '#' ? `${vals[usedValues++]}` : c))
    .join('');
};
