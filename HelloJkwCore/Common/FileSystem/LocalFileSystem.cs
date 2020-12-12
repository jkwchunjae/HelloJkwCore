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
        Encoding _encoding;

        public LocalFileSystem(Encoding encoding = null)
        {
            _encoding = encoding ?? new UTF8Encoding(false);
        }

        public Task<bool> FileExistsAsync(string path, CancellationToken ct = default)
        {
            return Task.FromResult(File.Exists(path));
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
