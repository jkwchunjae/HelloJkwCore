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

        public async Task<bool> CreateDirectoryAsync(string path, CancellationToken ct = default)
        {
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

        public async Task<bool> DeleteFileAsync(string path, CancellationToken ct = default)
        {
            await _client.Files.DeleteV2Async(path);
            return true;
        }

        public async Task<bool> DirExistsAsync(string path, CancellationToken ct = default)
        {
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

        public async Task<List<string>> GetFilesAsync(string path, string extension = null, CancellationToken ct = default)
        {
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
