using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.FileSystem
{
    public class LocalFileSystem : IFileSystem
    {
        protected readonly PathOf _pathOf;
        private readonly Encoding _encoding;

        public LocalFileSystem(PathOption pathOption, Encoding encoding = null)
        {
            _pathOf = new PathOf(pathOption, FileSystemType.Local);
            _encoding = encoding ?? new UTF8Encoding(false);
        }

        public Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            Directory.CreateDirectory(path);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            File.Delete(path);
            return Task.FromResult(true);
        }

        public Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            var exists = Directory.Exists(path);
            return Task.FromResult(exists);
        }

        public Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return Task.FromResult(File.Exists(path));
        }

        public Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            var list = Directory.GetFiles(path)
                .Select(x => System.IO.Path.GetFileName(x))
                .Where(x => extension == null || x.EndsWith(extension))
                .ToList();
            return Task.FromResult(list);
        }

        public PathOf GetPathOf()
        {
            return _pathOf;
        }

        public async Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            if (!File.Exists(path))
                return default(T);

            var text = await File.ReadAllTextAsync(path, _encoding);
            return Json.Deserialize<T>(text);
        }

        public async Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            var dir = Directory.GetParent(path);
            if (!Directory.Exists(dir.FullName))
            {
                Directory.CreateDirectory(dir.FullName);
            }
            await File.WriteAllTextAsync(path, Json.Serialize(obj), _encoding, ct);
            return true;
        }
    }
}
