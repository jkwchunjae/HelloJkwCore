using Common.Dropbox;
using Common.Extensions;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Common.FileSystem
{
    public class FileSystemService
    {
        private readonly Dictionary<FileSystemType, IFileSystem> _fsDic = new();

        public FileSystemService(FileSystemOption fsOption)
        {
            if (fsOption.Dropbox != null)
            {
                _fsDic.Add(FileSystemType.Dropbox, new DropboxFileSystem(DropboxHelper.GetDropboxClient(fsOption.Dropbox)));
            }
            _fsDic.Add(FileSystemType.Azure, new AzureFileSystem());
            _fsDic.Add(FileSystemType.InMemory, new InMemoryFileSystem());
            _fsDic.Add(FileSystemType.Local, new LocalFileSystem());
        }

        public IFileSystem GetFileSystem(FileSystemType fsType)
        {
            return _fsDic.ContainsKey(fsType) ? _fsDic[fsType] : null;
        }
    }
}
