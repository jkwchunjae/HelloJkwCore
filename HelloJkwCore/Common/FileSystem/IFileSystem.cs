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
    }
}
