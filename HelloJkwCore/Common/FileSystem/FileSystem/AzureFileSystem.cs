using Azure;
using Azure.Storage.Blobs;
using JkwExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class AzureFileSystem : IFileSystem
    {
        private Paths _pathOf;
        private string _connectionString;
        private Encoding _encoding;
        private ILogger<AzureFileSystem> _logger;

        private Dictionary<string, BlobContainerClient> _containerClients = new();

        public AzureFileSystem(
            PathMap pathOption,
            string connectionString,
            ILoggerFactory loggerFactory,
            Encoding encoding = null)
        {
            _pathOf = new Paths(pathOption, FileSystemType.Azure);
            _connectionString = connectionString;
            _encoding = encoding ?? new UTF8Encoding(false);
            _logger = loggerFactory?.CreateLogger<AzureFileSystem>();

            if (_logger != null)
            {
                _logger.LogDebug("AzureFileSystem Logger is working.");
            }
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
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
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
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
            _logger?.LogDebug("FileExistsAsync. container: {container}, path: {path}", containerName, path);
            var client = await GetContainerClient(containerName);

            var blobClient = client.GetBlobClient(path);
            var response = await blobClient.ExistsAsync();

            _logger?.LogDebug("FileExistsAsync. container: {container}, path: {path}, result: {result}", containerName, path, response.Value);
            return response.Value;
        }

        public async Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string extension = null, CancellationToken ct = default(CancellationToken))
        {
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
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

        public Paths GetPathOf()
        {
            return _pathOf;
        }

        public async Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default(CancellationToken))
        {
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
            _logger?.LogDebug("ReadJsonAsync. container: {container}, path: {path}", containerName, path);
            try
            {
                var client = await GetContainerClient(containerName);

                var blobClient = client.GetBlobClient(path);
                var response = await blobClient.DownloadAsync(ct);

                var reader = new StreamReader(response.Value.Content);
                string text = await reader.ReadToEndAsync();

                return Json.Deserialize<T>(text);
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, "[Error] ReadJsonAsync. container: {container}, path: {path}", containerName, path);
                return default(T);
            }
        }

        public async Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default(CancellationToken))
        {
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
            _logger?.LogDebug("WriteJsonAsync. container: {container}, path: {path}", containerName, path);
            try
            {
                var client = await GetContainerClient(containerName);
                var blobClient = client.GetBlobClient(path);

                var data = Json.Serialize(obj);
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
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
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
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
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
    }
}
