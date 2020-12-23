using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.FileSystem
{
    public class AzureFileSystem : IFileSystem
    {
        public Task<bool> CreateDirectoryAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFileAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirExistsAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExistsAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetFilesAsync(string path, string extension = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> ReadJsonAsync<T>(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteJsonAsync<T>(string path, T obj, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
