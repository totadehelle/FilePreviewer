using System;

namespace Caching
{
    public interface ICacheProvider : IDisposable
    {
        event Action OnCacheOverflow;
        object Get(string key);
        bool Contains(string key);
        void Add(string key, object value);
        void Remove(string key);
        void Trim(string[] itemsToKeep);
        void Clear();
    }
}