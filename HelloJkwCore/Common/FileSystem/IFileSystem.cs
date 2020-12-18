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
        Task<T> ReadJsonAsync<T>(string path, CancellationToken ct = default);
        Task<bool> WriteJsonAsync<T>(string path, T obj, CancellationToken ct = default);
        Task<bool> FileExistsAsync(string path, CancellationToken ct = default);
        Task<bool> DeleteFileAsync(string path, CancellationToken ct = default);
        Task<List<string>> GetFilesAsync(string path, string extension = null, CancellationToken ct = default);

        Task<bool> CreateDirectoryAsync(string path, CancellationToken ct = default);
        Task<bool> DirExistsAsync(string path, CancellationToken ct = default);
    }
}
