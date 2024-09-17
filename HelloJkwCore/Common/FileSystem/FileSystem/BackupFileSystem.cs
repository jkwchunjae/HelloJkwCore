namespace Common;

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

    public async Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        _backgroundQueue.QueueBackgroundWorkItem(async token =>
        {
            await _backup.CreateDirectoryAsync(pathFunc, token);
        });
        return await _fs.CreateDirectoryAsync(pathFunc, ct);
    }

    public async Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        _backgroundQueue.QueueBackgroundWorkItem(async token =>
        {
            await _backup.DeleteFileAsync(pathFunc, token);
        });
        return await _fs.DeleteFileAsync(pathFunc, ct);
    }

    public async Task<bool> DirExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        //_backgroundQueue.QueueBackgroundWorkItem(async token =>
        //{
        //    await _backup.DirExistsAsync(pathFunc, token);
        //});
        return await _fs.DirExistsAsync(pathFunc, ct);
    }

    public async Task<bool> FileExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        //_backgroundQueue.QueueBackgroundWorkItem(async token =>
        //{
        //    await _backup.FileExistsAsync(pathFunc, token);
        //});
        return await _fs.FileExistsAsync(pathFunc, ct);
    }

    public async Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string? extension = null, CancellationToken ct = default)
    {
        //_backgroundQueue.QueueBackgroundWorkItem(async token =>
        //{
        //    await _backup.GetFilesAsync(pathFunc, extension, token);
        //});
        return await _fs.GetFilesAsync(pathFunc, extension, ct);
    }

    public async Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        //_backgroundQueue.QueueBackgroundWorkItem(async token =>
        //{
        //    await _backup.ReadJsonAsync(pathFunc, token);
        //});
        return await _fs.ReadJsonAsync<T>(pathFunc, ct);
    }

    public async Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        return await _fs.ReadTextAsync(pathFunc, ct);
    }

    public async Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default)
    {
        _backgroundQueue.QueueBackgroundWorkItem(async token =>
        {
            await _backup.WriteJsonAsync(pathFunc, obj, token);
        });
        return await _fs.WriteJsonAsync(pathFunc, obj, ct);
    }

    public async Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string obj, CancellationToken ct = default)
    {
        _backgroundQueue.QueueBackgroundWorkItem(async token =>
        {
            await _backup.WriteTextAsync(pathFunc, obj, token);
        });
        return await _fs.WriteTextAsync(pathFunc, obj, ct);
    }

    public async Task<bool> WriteBlobAsync(Func<Paths, string> pathFunc, Stream stream, CancellationToken ct = default)
    {
        //_backgroundQueue.QueueBackgroundWorkItem(async token =>
        //{
        //    await _backup.WriteBlobAsync(pathFunc, stream, token);
        //});
        return await _fs.WriteBlobAsync(pathFunc, stream, ct);
    }

    public async Task<byte[]> ReadBlobAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        return await _fs.ReadBlobAsync(pathFunc, ct);
    }
}