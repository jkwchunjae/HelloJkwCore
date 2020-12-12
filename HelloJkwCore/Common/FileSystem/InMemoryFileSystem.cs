using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.FileSystem
{
    public class InMemoryFileSystem : IFileSystem
    {
        private Dictionary<string, string> _files = new();

        public Task<bool> FileExistsAsync(string path, CancellationToken ct = default)
        {
            return Task.FromResult(_files.ContainsKey(path));
        }

        public Task<T> ReadJsonAsync<T>(string path, CancellationToken ct = default)
        {
            var text = _files[path];
            return Task.FromResult(Json.Deserialize<T>(text));
        }

        public Task<bool> WriteJsonAsync<T>(string path, T obj, CancellationToken ct = default)
        {
            _files[path] = Json.Serialize(obj);
            return Task.FromResult(true);
        }
    }
}
