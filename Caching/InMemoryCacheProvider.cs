using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;


namespace Caching
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private MemoryCache _cache;
        private readonly int _cacheItemsLimit;
        readonly Queue<string> _indexedKeys;
        private event Action OnAddingCacheItem;

        public InMemoryCacheProvider(int cacheItemsLimit = 3)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _indexedKeys = new Queue<string>();
            _cacheItemsLimit = cacheItemsLimit;
            OnAddingCacheItem += CacheRoller;
        }

        private void CacheRoller()
        {
            while (_cache.Count > _cacheItemsLimit)
            {
                var theOldestKey = _indexedKeys.Dequeue();
                Remove(theOldestKey);
            }
        }

        public object Get(string key)
        {
            var content = _cache.Get(key);
            return content;
        }

        public void Add(string key, object value)
        {
            var options = new MemoryCacheEntryOptions();
            _cache.Set(key, value);
            _indexedKeys.Enqueue(key);
            OnAddingCacheItem?.Invoke();
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            var newCache = new MemoryCache(new MemoryCacheOptions());
            var oldCache = _cache;
            _cache = newCache;
            oldCache.Dispose();
        }
    }
}