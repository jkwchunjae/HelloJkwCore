using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class CacheData<T>
    {
        public T Data { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class MemoryCache<T> : ICache<T>
    {
        private Dictionary<string, CacheData<T>> _cached;

        public MemoryCache()
        {
            _cached = new Dictionary<string, CacheData<T>>();
        }

        public bool Contains(string key)
        {
            if (_cached.TryGetValue(key, out var data))
            {
                if (data.ExpiredAt == null)
                    return true;
                if (data.ExpiredAt > DateTime.Now)
                    return true;
            }
            return false;
        }

        public void Set(string key, T value, TimeSpan? ttl = null)
        {
            _cached[key] = new CacheData<T>
            {
                Data = value,
                ExpiredAt = ttl.HasValue ? DateTime.Now + ttl.Value : (DateTime?)null,
            };
        }

        public T Get(string key)
            => Contains(key) ? _cached[key].Data : default;

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
