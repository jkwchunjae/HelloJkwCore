using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class InMemoryFileSystem : IFileSystem
    {
        protected readonly PathOf _pathOf;
        private readonly Dictionary<string, string> _files = new();

        public InMemoryFileSystem(PathOption pathOption)
        {
            _pathOf = new PathOf(pathOption, FileSystemType.InMemory);
        }

        public Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            if (_files.ContainsKey(path))
                _files.Remove(path);

            return Task.FromResult(true);
        }

        public Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            return Task.FromResult(true);
        }

        public Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return Task.FromResult(_files.ContainsKey(path));
        }

        public Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
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

        public PathOf GetPathOf()
        {
            return _pathOf;
        }

        public Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());

            if (!_files.ContainsKey(path))
                return Task.FromResult(default(T));

            var text = _files[path];
            return Task.FromResult(Json.Deserialize<T>(text));
        }

        public Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            _files[path] = Json.Serialize(obj);
            return Task.FromResult(true);
        }
    }
}
