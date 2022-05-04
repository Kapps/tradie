export const getRandomElement = <T>(array: Array<T>) => {
  return array[Math.floor(Math.random() * array.length)];
};

export const getNRandomElements = <T>(array: Array<T>, n: number) => {
  const clone = [...array];
  for (let i = clone.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [clone[i], clone[j]] = [clone[j], clone[i]];
  }
  return array.slice(0, n);
};