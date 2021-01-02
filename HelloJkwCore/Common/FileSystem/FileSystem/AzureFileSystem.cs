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
        private PathOf _pathOf;
        private string _connectionString;
        private Encoding _encoding;
        private ILogger<AzureFileSystem> _logger;

        private Dictionary<string, BlobContainerClient> _containerClients = new();

        public AzureFileSystem(
            PathOption pathOption,
            string connectionString,
            ILoggerFactory loggerFactory,
            Encoding encoding = null)
        {
            _pathOf = new PathOf(pathOption, FileSystemType.Azure);
            _connectionString = connectionString;
            _encoding = encoding ?? new UTF8Encoding(false);
            _logger = loggerFactory?.CreateLogger<AzureFileSystem>();
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


        public Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default(CancellationToken))
        {
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
            var client = await GetContainerClient(containerName);

            await client.DeleteBlobIfExistsAsync(path);
            return true;
        }

        public Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        public async Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default(CancellationToken))
        {
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
            var client = await GetContainerClient(containerName);

            var blobClient = client.GetBlobClient(path);
            var response = await blobClient.ExistsAsync();

            return response.Value;
        }

        public async Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default(CancellationToken))
        {
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
            var client = await GetContainerClient(containerName);

            var result = new List<string>();

            if (!path.EndsWith("/"))
                path += "/";

            await foreach (var item in client.GetBlobsAsync(prefix: path))
            {
                result.Add(Path.GetFileName(item.Name));
            }
            return result;
        }

        public PathOf GetPathOf()
        {
            return _pathOf;
        }

        public async Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default(CancellationToken))
        {
            var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
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
                _logger.LogError(ex, "ReadJsonAsync. container: {container}, path: {path}", containerName, path);
                return default(T);
            }
        }

        public async Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                var (containerName, path) = ParsePath(pathFunc(GetPathOf()));
                var client = await GetContainerClient(containerName);

                var blobClient = client.GetBlobClient(path);
                await blobClient.DeleteIfExistsAsync();

                var data = Json.Serialize(obj);
                using (var stream = new MemoryStream(_encoding.GetBytes(data)))
                {
                    var response = await client.UploadBlobAsync(path, stream);
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
