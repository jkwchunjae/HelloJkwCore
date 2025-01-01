using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public class AzureFileSystemBuilder : IFileSystemBuilder
{
    public FileSystemType FileSystemType => FileSystemType.Azure;

    private readonly string _connectionString;
    private readonly ISerializer _serializer;
    private readonly Encoding _encoding;
    private readonly ILoggerFactory _loggerFactory;

    public AzureFileSystemBuilder(
        string connectionString,
        ISerializer serializer,
        ILoggerFactory loggerFactory,
        Encoding? encoding = null)
    {
        _connectionString = connectionString;
        _serializer = serializer;
        _encoding = encoding ?? new UTF8Encoding(false);
        _loggerFactory = loggerFactory;
    }

    public IFileSystem Build(PathMap pathMap)
    {
        var paths = new Paths(pathMap, FileSystemType.Azure);
        return new AzureFileSystem(paths, _connectionString, _serializer, _loggerFactory, _encoding);
    }
}

public static class AzureFileSystemExtensions
{
    public static IServiceCollection AddAzureFileSystem(this IServiceCollection services, AzureOption azureOption)
    {
        services.AddSingleton<IFileSystemBuilder, AzureFileSystemBuilder>(serviceProvider =>
        {
            var serializer = serviceProvider.GetRequiredService<ISerializer>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new AzureFileSystemBuilder(azureOption.ConnectionString, serializer, loggerFactory);
        });
        return services;
    }
}

public class AzureFileSystem : IFileSystem
{
    public FileSystemType FileSystemType => FileSystemType.Azure;
    private readonly Paths _paths;
    private readonly string _connectionString;
    private readonly ISerializer _serializer;
    private readonly Encoding _encoding;
    private readonly ILogger<AzureFileSystem> _logger;
    private readonly Dictionary<string, BlobContainerClient> _containerClients = new();


    public AzureFileSystem(
        Paths paths,
        string connectionString,
        ISerializer serializer,
        ILoggerFactory loggerFactory,
        Encoding? encoding = null)
    {
        _paths = paths;
        _connectionString = connectionString;
        _serializer = serializer;
        _encoding = encoding ?? new UTF8Encoding(false);
        _logger = loggerFactory.CreateLogger<AzureFileSystem>();
    }

    private async Task<BlobContainerClient> GetContainerClient(string containerName)
    {
        if (!_containerClients.ContainsKey(containerName))
        {
            try
            {
                var client = new BlobContainerClient(_connectionString, containerName);
                _containerClients[containerName] = client;

                if (!await client.ExistsAsync())
                {
                    _logger?.LogInformation("Create container. name: {containerName}", containerName);
                    await client.CreateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in Create container. name: {containerName}, connStr: {connectionString}", containerName, _connectionString);
                throw;
            }
        }

        return _containerClients[containerName];
    }

    private (string ContainerName, string Path) ParsePath(string path)
    {
        var arr = path.Split(':');

        if (arr.Length != 3)
            throw new Exception($"Invalid azure path {path}");

        var containerName = arr[0];
        var azurePath = arr[2].RegexReplace("^/*", "");

        return (containerName, azurePath);
    }


    public Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default(CancellationToken))
    {
        return Task.FromResult(true);
    }

    public async Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default(CancellationToken))
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        var client = await GetContainerClient(containerName);

        await client.DeleteBlobIfExistsAsync(path);
        return true;
    }

    public Task<bool> DirExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default(CancellationToken))
    {
        return Task.FromResult(true);
    }

    public async Task<bool> FileExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default(CancellationToken))
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("FileExistsAsync. container: {container}, path: {path}", containerName, path);
        var client = await GetContainerClient(containerName);

        var blobClient = client.GetBlobClient(path);
        var response = await blobClient.ExistsAsync();

        _logger?.LogDebug("FileExistsAsync. container: {container}, path: {path}, result: {result}", containerName, path, response.Value);
        return response.Value;
    }

    public async Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string? extension = null, CancellationToken ct = default(CancellationToken))
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("GetFilesAsync. container: {container}, path: {path}", containerName, path);
        var client = await GetContainerClient(containerName);

        var result = new List<string>();

        if (!path.EndsWith("/"))
            path += "/";

        await foreach (var item in client.GetBlobsAsync(prefix: path))
        {
            result.Add(Path.GetFileName(item.Name));
        }
        _logger?.LogDebug("GetFilesAsync. container: {container}, path: {path}, result: [{result}]", containerName, path, result.StringJoin(","));
        return result;
    }

    public async Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default(CancellationToken))
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("ReadJsonAsync. container: {container}, path: {path}", containerName, path);
        try
        {
            var client = await GetContainerClient(containerName);

            var blobClient = client.GetBlobClient(path);
            var response = await blobClient.DownloadAsync(ct);

            var reader = new StreamReader(response.Value.Content);
            string text = await reader.ReadToEndAsync();

            return _serializer.Deserialize<T>(text);
        }
        catch (RequestFailedException ex)
        {
            _logger?.LogError(ex, "[Error] ReadJsonAsync. container: {container}, path: {path}", containerName, path);
            throw new Exception($"Error in ReadJsonAsync. container: {containerName}, path: {path}", ex);
        }
    }

    public async Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default(CancellationToken))
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("WriteJsonAsync. container: {container}, path: {path}", containerName, path);
        try
        {
            var client = await GetContainerClient(containerName);
            var blobClient = client.GetBlobClient(path);

            var data = _serializer.Serialize(obj);
            using (var stream = new MemoryStream(_encoding.GetBytes(data)))
            {
                var response = await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[Error] WriteJsonAsync. container: {container}, path: {path}", containerName, path);
            throw;
        }
    }

    public async Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("ReadTextAsync. container: {container}, path: {path}", containerName, path);
        try
        {
            var client = await GetContainerClient(containerName);

            var blobClient = client.GetBlobClient(path);
            var response = await blobClient.DownloadAsync(ct);

            var reader = new StreamReader(response.Value.Content);
            string text = await reader.ReadToEndAsync();

            return text;
        }
        catch (RequestFailedException ex)
        {
            _logger?.LogError(ex, "[Error] ReadTextAsync. container: {container}, path: {path}", containerName, path);
            return string.Empty;
        }
    }

    public async Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string text, CancellationToken ct = default)
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("WriteTextAsync. container: {container}, path: {path}", containerName, path);
        try
        {
            var client = await GetContainerClient(containerName);
            var blobClient = client.GetBlobClient(path);

            using (var stream = new MemoryStream(_encoding.GetBytes(text)))
            {
                var response = await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[Error] WriteTextAsync. container: {container}, path: {path}", containerName, path);
            throw;
        }
    }

    public async Task<bool> WriteBlobAsync(Func<Paths, string> pathFunc, Stream stream, CancellationToken ct = default)
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("WriteBlobAsync. container: {container}, path: {path}", containerName, path);
        try
        {
            var client = await GetContainerClient(containerName);
            var blobClient = client.GetBlobClient(path);

            var response = await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[Error] WriteBlobAsync. container: {container}, path: {path}", containerName, path);
            throw;
        }
    }

    public async Task<byte[]> ReadBlobAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("ReadBlobAsync. container: {container}, path: {path}", containerName, path);
        try
        {
            var client = await GetContainerClient(containerName);
            var blobClient = client.GetBlobClient(path);

            var content = await blobClient.DownloadContentAsync(ct);
            var bytes = content.Value.Content.ToArray();
            return bytes;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[Error] ReadBlobAsync. container: {container}, path: {path}", containerName, path);
            throw;
        }
    }

    public async Task<string> GenerateSasUrlAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var (containerName, path) = ParsePath(pathFunc(_paths));
        _logger?.LogDebug("GenerateSasUrlAsync. container: {container}, path: {path}", containerName, path);
        try
        {
            var client = await GetContainerClient(containerName);
            var blobClient = client.GetBlobClient(path);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = path,
                Resource = "b", // Blob에 대한 SAS
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var uri = blobClient.GenerateSasUri(sasBuilder);
            _logger?.LogInformation("GenerateSasUrlAsync. container: {container}, path: {path}, uri: {uri}", containerName, path, uri);

            return uri.ToString();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[Error] GenerateSasUrlAsync. container: {container}, path: {path}", containerName, path);
            throw;
        }
    }
}