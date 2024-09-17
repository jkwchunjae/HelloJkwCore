using Dropbox.Api;
using Dropbox.Api.Files;

namespace Common;

public class DropboxFileSystem : IFileSystem
{
    protected readonly Paths _paths;
    private readonly DropboxClient _client;
    private readonly Encoding _encoding;

    public DropboxFileSystem(PathMap pathOption, DropboxClient client, Encoding? encoding = null)
    {
        _paths = new Paths(pathOption, FileSystemType.Dropbox);
        _client = client;
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
        catch (ApiException<CreateFolderError>)
        {
            return false;
        }
        catch
        {
            return false;
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
        catch (ApiException<GetMetadataError>)
        {
            return false;
        }
        catch
        {
            return false;
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
        catch (ApiException<GetMetadataError>)
        {
            return false;
        }
        catch
        {
            return false;
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
            return await _client.ReadJsonAsync<T>(path);
        }
        catch
        {
            throw;
        }
    }

    public async Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            return await _client.ReadTextAsync(path);
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        try
        {
            await _client.WriteJsonAsync(path, obj, _encoding);
            return true;
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
            await _client.WriteTextAsync(path, text, _encoding);
            return true;
        }
        catch (ApiException<UploadError>)
        {
            return false;
        }
        catch
        {
            return false;
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
        catch (ApiException<UploadError>)
        {
            return false;
        }
        catch
        {
            return false;
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