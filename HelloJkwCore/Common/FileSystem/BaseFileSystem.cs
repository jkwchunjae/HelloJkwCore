using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.FileSystem
{
    public abstract class BaseFileSystem : IFileSystem
    {

        protected readonly PathOf _pathOf;

        protected BaseFileSystem(PathOption pathOption, FileSystemType fsType)
        {
            _pathOf = new PathOf(pathOption, fsType);
        }

        public virtual Task<bool> CreateDirectoryAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return CreateDirectoryAsync(path, ct);
        }

        public virtual Task<bool> DeleteFileAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return DeleteFileAsync(path, ct);
        }

        public virtual Task<bool> DirExistsAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return DirExistsAsync(path, ct);
        }

        public virtual Task<bool> FileExistsAsync(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return FileExistsAsync(path, ct);
        }

        public virtual Task<List<string>> GetFilesAsync(string path, string extension = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return GetFilesAsync(path, extension, ct);
        }

        public virtual PathOf GetPathOf()
        {
            return _pathOf;
        }

        public virtual Task<T> ReadJsonAsync<T>(string path, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return ReadJsonAsync<T>(path, ct);
        }

        public virtual Task<bool> WriteJsonAsync<T>(string path, T obj, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            return WriteJsonAsync(path, obj, ct);
        }
    }
}
