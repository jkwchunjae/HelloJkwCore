using Common.Dropbox;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Common.FileSystem
{
    public interface IFileSystemService
    {
        IFileSystem GetFileSystem(FileSystemType fsType);
    }

    public class FileSystemService : IFileSystemService
    {
        private readonly Dictionary<FileSystemType, IFileSystem> _fsDic = new();

        public FileSystemService(FileSystemOption fsOption, PathOption pathOption)
        {
            if (fsOption.Dropbox != null)
            {
                var dropboxClient = DropboxHelper.GetDropboxClient(fsOption.Dropbox);
                _fsDic.Add(FileSystemType.Dropbox, new DropboxFileSystem(pathOption, dropboxClient));
            }
            _fsDic.Add(FileSystemType.Azure, new AzureFileSystem(pathOption));
            _fsDic.Add(FileSystemType.InMemory, new InMemoryFileSystem(pathOption));
            _fsDic.Add(FileSystemType.Local, new LocalFileSystem(pathOption));
        }

        public IFileSystem GetFileSystem(FileSystemType fsType)
        {
            return _fsDic.ContainsKey(fsType) ? _fsDic[fsType] : null;
        }
    }
}
