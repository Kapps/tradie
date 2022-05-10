const cachedResources = new Map<string, unknown>();
const loadPromises = new Map<string, unknown>();

/**
 * Returns a cached version of the resource with the given key.
 * Otherwise, it will fetch the resource and cache it.
 * @param key The key to use for the cache
 * @param fn The function to call to get the resource
 * @returns
 */
export const memoizeLocalStorage = async <T>(key: string, fn: () => Promise<T>) => {
  if (loadPromises.has(key)) {
    return loadPromises.get(key);
  }

  const executor = async () => {
    if (cachedResources.has(key)) {
      console.log(`Returning memory cached resource ${key}`);
      return <T>cachedResources.get(key);
    }

    const CACHE_KEY = `CACHED_${key.toUpperCase()}`;
    const CACHE_TTL = 1000 * 60 * 60 * 24;

    const cachedItems = JSON.parse(window.localStorage.getItem(CACHE_KEY) ?? 'null');
    if (cachedItems) {
      if (cachedItems.timestamp && Date.now() - cachedItems.timestamp < CACHE_TTL) {
        console.log(`Returning cached resource ${key}`);
        return cachedItems.data;
      }
    }

    console.log(`Skiping cache for ${key}`);

    const data = await fn();
    const serialized = JSON.stringify({
      data,
      timestamp: Date.now(),
    });

    window.localStorage.setItem(CACHE_KEY, serialized);
    cachedResources.set(key, data);

    return data;
  };

  console.log('executing promise');
  const promise = executor();
  loadPromises.set(key, promise);
  return await promise;
};
