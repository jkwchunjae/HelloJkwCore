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
        private readonly Dictionary<string, string> _files = new();

        public Task<bool> DeleteFileAsync(string path, CancellationToken ct = default)
        {
            if (_files.ContainsKey(path))
                _files.Remove(path);

            return Task.FromResult(true);
        }

        public Task<bool> FileExistsAsync(string path, CancellationToken ct = default)
        {
            return Task.FromResult(_files.ContainsKey(path));
        }

        public Task<List<string>> GetFilesAsync(string path, string extension = null)
        {
            if (!path.EndsWith("/"))
                path += "/";

            var list = _files.Keys
                .Where(x => x.StartsWith(path))
                .Select(x => x.Replace(path, ""))
                // path를 지웠는데 '/'가 있으면 file이 아니다.
                .Where(x => !x.Contains("/"))
                .Where(x => extension == null || x.EndsWith(extension))
                .ToList();

            return Task.FromResult(list);
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
