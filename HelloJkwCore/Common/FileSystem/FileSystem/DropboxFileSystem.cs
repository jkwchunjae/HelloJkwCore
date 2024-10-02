using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public class DropboxFileSystemBuilder : IFileSystemBuilder
{
    public FileSystemType FileSystemType => FileSystemType.Dropbox;

    private readonly DropboxClient _client;
    private readonly ISerializer _serializer;
    private readonly Encoding _encoding;

    public DropboxFileSystemBuilder(
        DropboxClient client,
        ISerializer serializer,
        Encoding? encoding = null
    )
    {
        _client = client;
        _serializer = serializer;
        _encoding = encoding ?? new UTF8Encoding(false);
    }

    public IFileSystem Build(PathMap pathMap)
    {
        var paths = new Paths(pathMap, FileSystemType.Dropbox);
        return new DropboxFileSystem(paths, _client, _serializer, _encoding);
    }
}

public static class DropboxFileSystemExtensions
{
    public static IServiceCollection AddDropboxFileSystem(this IServiceCollection services, DropboxOption dropboxOption)
    {
        services.AddSingleton<IFileSystemBuilder, DropboxFileSystemBuilder>(serviceProvider =>
        {
            var dropboxClient = new DropboxClient(
                dropboxOption.RefreshToken,
                dropboxOption.ClientId,
                dropboxOption.ClientSecret);
            var serializer = serviceProvider.GetRequiredService<ISerializer>();
            return new DropboxFileSystemBuilder(dropboxClient, serializer);
        });
        return services;
    }
}

public class DropboxFileSystem : IFileSystem
{
    public FileSystemType FileSystemType => FileSystemType.Dropbox;
    private readonly Paths _paths;
    private readonly DropboxClient _client;
    private readonly Encoding _encoding;
    private readonly ISerializer _serializer;

    public DropboxFileSystem(
        Paths paths,
        DropboxClient client,
        ISerializer serializer,
        Encoding? encoding = null)
    {
        _paths = paths;
        _client = client;
        _serializer = serializer;
        _encoding = encoding ?? new UTF8Encoding(false);
    }

    public async Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);

        try
        {
            await _client.Files.CreateFolderV2Async(path);
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        await _client.Files.DeleteV2Async(path);
        return true;
    }

    public async Task<bool> DirExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            var metadata = await _client.Files.GetMetadataAsync(path);
            return metadata.IsFolder;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            var metadata = await _client.Files.GetMetadataAsync(path);
            return metadata.IsFile;
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string? extension = null, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        var fileMetadataList = new List<Metadata>();

        var result = await _client.Files.ListFolderAsync(path);
        fileMetadataList.AddRange(result.Entries);

        while (result.HasMore)
        {
            result = await _client.Files.ListFolderContinueAsync(result.Cursor);
            fileMetadataList.AddRange(result.Entries);
        }

        return fileMetadataList
            .Where(x => x.IsFile)
            .Select(x => x.Name)
            .Where(x => extension == null || x.EndsWith(extension))
            .ToList();
    }

    public async Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            var text = await ReadTextAsync(pathFunc, ct);
            var parsed = _serializer.Deserialize<T>(text);
            return parsed;
        }
        catch
        {
            throw;
        }
    }

    public async Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        try
        {
            var path = pathFunc(_paths);
            var fileMetadata = await _client.Files.DownloadAsync(path);
            var fileText = await fileMetadata.GetContentAsStringAsync();
            return fileText;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default)
    {
        try
        {
            var path = pathFunc(_paths);
            var text = _serializer.Serialize(obj);
            return await WriteTextAsync(pathFunc, text, ct);
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string text, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            var bytes = _encoding.GetBytes(text);
            using var stream = new MemoryStream(bytes);
            var uploadArgs = new UploadArg(path, mode: WriteMode.Overwrite.Instance);
            await _client.Files.UploadAsync(uploadArgs, stream);
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> WriteBlobAsync(Func<Paths, string> pathFunc, Stream stream, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            await _client.Files.UploadAsync(path, WriteMode.Overwrite.Instance, body: stream);
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<byte[]> ReadBlobAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            using var response = await _client.Files.DownloadAsync(path);
            return await response.GetContentAsByteArrayAsync();
        }
        catch
        {
            throw;
        }
    }
}