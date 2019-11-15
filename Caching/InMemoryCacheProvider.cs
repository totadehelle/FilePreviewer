using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;


namespace Caching
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private MemoryCache _cache;
        public int CacheItemsLimit { get; set; } = 3;
        private readonly HashSet<string> _cacheKeys;
        public event Action OnCacheOverflow;

        public InMemoryCacheProvider()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacheKeys = new HashSet<string>();
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public bool Contains(string key)
        {
            return _cacheKeys.Contains(key);
        }

        public void Add(string key, object value)
        {
            _cache.Set(key, value);
            _cacheKeys.Add(key);
            if(_cache.Count > CacheItemsLimit)
                OnCacheOverflow?.Invoke();
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
        }

        public void Clear()
        {
            var oldCache = Interlocked.Exchange(ref _cache, new MemoryCache(new MemoryCacheOptions()));
            oldCache.Dispose();
        }

        public void Trim(string[] itemsToKeep)
        {
            var itemsToRemove = _cacheKeys.Except(itemsToKeep).ToArray();
            foreach (var key in itemsToRemove)
            {
                Remove(key);
            }
        }
        
        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}