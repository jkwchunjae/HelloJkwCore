using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class DropboxFileSystem : IFileSystem
    {
        protected readonly PathOf _pathOf;
        private readonly DropboxClient _client;
        private readonly Encoding _encoding;

        public DropboxFileSystem(PathOption pathOption, DropboxClient client, Encoding encoding = null)
        {
            _pathOf = new PathOf(pathOption, FileSystemType.Dropbox);
            _client = client;
            _encoding = encoding ?? new UTF8Encoding(false);
        }

        public async Task<bool> CreateDirectoryAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());

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

        public async Task<bool> DeleteFileAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
            await _client.Files.DeleteV2Async(path);
            return true;
        }

        public async Task<bool> DirExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
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

        public async Task<bool> FileExistsAsync(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
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

        public async Task<List<string>> GetFilesAsync(Func<PathOf, string> pathFunc, string extension = null, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
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

        public PathOf GetPathOf()
        {
            return _pathOf;
        }

        public async Task<T> ReadJsonAsync<T>(Func<PathOf, string> pathFunc, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
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

        public async Task<bool> WriteJsonAsync<T>(Func<PathOf, string> pathFunc, T obj, CancellationToken ct = default)
        {
            var path = pathFunc(GetPathOf());
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
