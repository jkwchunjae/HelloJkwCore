using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class MemoryCache<T> : ICache<T>
    {
        private Dictionary<string, T> _cached;

        public MemoryCache()
        {
            _cached = new Dictionary<string, T>();
        }

        public bool Contains(string key)
            => _cached.ContainsKey(key);

        public void Set(string key, T value)
        {
            _cached[key] = value;
        }

        public T Get(string key)
            => Contains(key) ? _cached[key] : default;

        public bool TryGet(string key, out T value)
        {
            if (Contains(key))
            {
                value = Get(key);
                return true;
            }

            value = default;
            return false;
        }
    }
}
