using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.FileSystem
{
    public class BackupFileSystem : IFileSystem
    {
        private readonly IBackgroundTaskQueue _backgroundQueue;

        private readonly IFileSystem _fs;
        private readonly IFileSystem _backup;

        public BackupFileSystem(IFileSystem fsMain, IFileSystem fsBackup, IBackgroundTaskQueue backgroundQueue)
        {
            _backgroundQueue = backgroundQueue;

            _fs = fsMain;
            _backup = fsBackup;
        }

        public async Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            _backgroundQueue.QueueBackgroundWorkItem(async token =>
            {
                await _backup.CreateDirectoryAsync(pathFunc, token);
            });
            return await _fs.CreateDirectoryAsync(pathFunc, ct);
        }

        public async Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            _backgroundQueue.QueueBackgroundWorkItem(async token =>
            {
                await _backup.DeleteFileAsync(pathFunc, token);
            });
            return await _fs.DeleteFileAsync(pathFunc, ct);
        }

        public async Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            //_backgroundQueue.QueueBackgroundWorkItem(async token =>
            //{
            //    await _backup.DirExistsAsync(pathFunc, token);
            //});
            return await _fs.DirExistsAsync(pathFunc, ct);
        }

        public async Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            //_backgroundQueue.QueueBackgroundWorkItem(async token =>
            //{
            //    await _backup.FileExistsAsync(pathFunc, token);
            //});
            return await _fs.FileExistsAsync(pathFunc, ct);
        }

        public async Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default)
        {
            //_backgroundQueue.QueueBackgroundWorkItem(async token =>
            //{
            //    await _backup.GetFilesAsync(pathFunc, extension, token);
            //});
            return await _fs.GetFilesAsync(pathFunc, extension, ct);
        }

        public PathOf GetPathOf()
        {
            return _fs.GetPathOf();
        }

        public async Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            //_backgroundQueue.QueueBackgroundWorkItem(async token =>
            //{
            //    await _backup.ReadJsonAsync(pathFunc, token);
            //});
            return await _fs.ReadJsonAsync<T>(pathFunc, ct);
        }

        public async Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default)
        {
            _backgroundQueue.QueueBackgroundWorkItem(async token =>
            {
                await _backup.WriteJsonAsync(pathFunc, obj, token);
            });
            return await _fs.WriteJsonAsync(pathFunc, obj, ct);
        }
    }
}
