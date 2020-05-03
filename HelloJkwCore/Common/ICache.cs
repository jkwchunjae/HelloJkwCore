using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public interface ICache<T>
    {
        public bool Contains(string key);

        public void Set(string key, T value);

        public T Get(string key);

        public bool TryGet(string key, out T value);
    }
}
