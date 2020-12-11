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
            catch (ApiException<GetMetadataError> ex)
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
            return await _client.ReadJsonAsync<T>(path);
        }

        public async Task<bool> WriteJsonAsync<T>(string path, T obj, CancellationToken ct = default)
        {
            try
            {
                var text = Json.Serialize(obj);
                var bytes = _encoding.GetBytes(text);
                using (var stream = new MemoryStream(bytes))
                {
                    var uploadResult = await _client.Files.UploadAsync(
                        commitInfo: new CommitInfo(path: path, mode: Overwrite.Instance),
                        body: stream);
                    return true;
                }
            }
            catch (ApiException<UploadError> ex)
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
