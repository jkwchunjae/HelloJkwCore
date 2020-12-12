using Common.Extensions;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Dropbox.Api.Files.WriteMode;

namespace Common.FileSystem
{
    public class DropboxFileSystem : IFileSystem
    {
        private readonly DropboxClient _client;
        private readonly Encoding _encoding;

        public DropboxFileSystem(DropboxClient client, Encoding encoding = null)
        {
            _client = client;
            _encoding = encoding ?? new UTF8Encoding(false);
        }

        public async Task<bool> FileExistsAsync(string path, CancellationToken ct = default)
        {
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

        public async Task<T> ReadJsonAsync<T>(string path, CancellationToken ct = default)
        {
            try
            {
                return await _client.ReadJsonAsync<T>(path);
            }
            catch (ApiException<DownloadError>)
            {
                return default;
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> WriteJsonAsync<T>(string path, T obj, CancellationToken ct = default)
        {
            try
            {
                await _client.WriteJsonAsync(path, obj, _encoding);
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
    }
}
