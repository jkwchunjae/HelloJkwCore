﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common;

public interface IFileSystem
{
    Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default);
    Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string text, CancellationToken ct = default);
    Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default);
    Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default);
    Task<bool> FileExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default);
    Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default);
    Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string extension = null, CancellationToken ct = default);

    Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default);
    Task<bool> DirExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default);
}