/**
 * Returns a human readable description of the two numbers as a range.
 */
export const getRangeDescription = (minValue?: number, maxValue?: number, placeholder = '#') => {
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
  return placeholder;
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
    return text.replace(/#/, `${getRangeDescription(rounded(min), rounded(max))}`);
  }
  let usedValues = 0;
  const vals = [rounded(min) ?? '#', rounded(max) ?? '#'];
  return text
    .split('')
    .map((c) => (c === '#' ? `${vals[usedValues++]}` : c))
    .join('');
};

/**
 * Returns a properly rounded version of the given number, or NaN if the number is undefined.
 * @example rounded(0.5) => 0.5
 * @example rounded(0.5121312) => 0.512
 */
export const rounded = (value?: number) => value === undefined ? NaN : Math.round(value * 100 - 1) / 100;

/**
 * Extracts the range of numbers from a string.
 * @example '>=1' => [1, undefined]
 * @example '<=1' => [undefined, 1]
 * @example '1-2' => [1, 2]
 * @example '1' => [1, undefined]
 * @example '' => [undefined, undefined]
 */
export const extractRange = (text: string) => {
  if (!text) {
    return { success: true, min: undefined, max: undefined };
  }
  let match = text.match(/^(?:>=)?(\d+)-(?:<=)?(\d+)$/);
  if (match) {
    return { success: true, min: parseInt(match[1], 10), max: parseInt(match[2], 10) };
  }

  match = text.match(/^(?:>=?)?(\d+)$/);
  if (match) {
    return { success: true, min: parseInt(match[1], 10), max: undefined };
  }

  match = text.match(/^(?:<=?)?(\d+)$/);
  if (match) {
    return { success: true, min: parseInt(match[1], 10), max: undefined };
  }


  return { success: false, min: undefined, max: undefined };
};
