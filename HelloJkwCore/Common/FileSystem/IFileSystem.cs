using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.FileSystem
{
    public interface IFileSystem
    {
        PathOf GetPathOf();
        Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default);
        Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default);
        Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default);
        Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default);
        Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default);

        Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default);
        Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default);
    }
}
