namespace Caching
{
    public interface ICacheProvider
    {
        object Get(string key);
        void Add(string key, object value);
        void Remove(string key);
        void Clear();
    }
}