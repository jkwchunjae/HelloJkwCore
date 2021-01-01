using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class AzureFileSystem : IFileSystem
    {
        protected readonly PathOf _pathOf;

        public AzureFileSystem(PathOption pathOption)
        {
            _pathOf = new PathOf(pathOption, FileSystemType.Azure);
        }

        public Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public PathOf GetPathOf()
        {
            return _pathOf;
        }

        public Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
