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
        private readonly Encoding _encoding;

        public LocalFileSystem(Encoding encoding = null)
        {
            _encoding = encoding ?? new UTF8Encoding(false);
        }

        public Task<bool> CreateDirectoryAsync(string path, CancellationToken ct = default)
        {
            Directory.CreateDirectory(path);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteFileAsync(string path, CancellationToken ct = default)
        {
            File.Delete(path);
            return Task.FromResult(true);
        }

        public Task<bool> DirExistsAsync(string path, CancellationToken ct = default)
        {
            var exists = Directory.Exists(path);
            return Task.FromResult(exists);
        }

        public Task<bool> FileExistsAsync(string path, CancellationToken ct = default)
        {
            return Task.FromResult(File.Exists(path));
        }

        public Task<List<string>> GetFilesAsync(string path, string extension = null, CancellationToken ct = default)
        {
            var list = Directory.GetFiles(path)
                .Select(x => Path.GetFileName(x))
                .Where(x => extension == null || x.EndsWith(extension))
                .ToList();
            return Task.FromResult(list);
        }

        public async Task<T> ReadJsonAsync<T>(string path, CancellationToken ct = default)
        {
            var text = await File.ReadAllTextAsync(path, _encoding);
            return Json.Deserialize<T>(text);
        }

        public async Task<bool> WriteJsonAsync<T>(string path, T obj, CancellationToken ct = default)
        {
            await File.WriteAllTextAsync(path, Json.Serialize(obj), _encoding, ct);
            return true;
        }
    }
}
