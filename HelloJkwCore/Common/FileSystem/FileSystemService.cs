using Common.Dropbox;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public interface IFileSystemService
    {
        IFileSystem MainFileSystem { get; }
        IFileSystem GetFileSystem(FileSystemType fsType);
        IFileSystem GetFileSystem(FileSystemSelectOption fileSystemSelectOption);
    }

    public class FileSystemService : IFileSystemService
    {
        private readonly Dictionary<FileSystemType, IFileSystem> _fsDic = new();
        private readonly ILogger<FileSystemService> _logger;

        public IFileSystem MainFileSystem { get; private set; }

        public FileSystemService(
            FileSystemOption fsOption,
            PathOption pathOption,
            IBackgroundTaskQueue backgroundTaskQueue,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<FileSystemService>();

            if (fsOption.Dropbox != null)
            {
                var dropboxClient = DropboxHelper.GetDropboxClient(fsOption.Dropbox);
                _fsDic.Add(FileSystemType.Dropbox, new DropboxFileSystem(pathOption, dropboxClient));
                _logger?.LogInformation("Create dropbox filesystem.");
            }
            if (fsOption.Azure != null)
            {
                _fsDic.Add(FileSystemType.Azure, new AzureFileSystem(pathOption, fsOption.Azure.ConnectionString, loggerFactory));
                _logger?.LogInformation("Create azure filesystem.");
            }
            _fsDic.Add(FileSystemType.InMemory, new InMemoryFileSystem(pathOption));
            _fsDic.Add(FileSystemType.Local, new LocalFileSystem(pathOption));

            MainFileSystem = CreateMainFileSystem(fsOption, backgroundTaskQueue);
        }

        private IFileSystem CreateMainFileSystem(FileSystemOption fsOption, IBackgroundTaskQueue backgroundTaskQueue)
        {
            if (fsOption.MainFileSystem.UseBackup)
            {
                var fsMain = GetFileSystem(fsOption.MainFileSystem.MainFileSystem);
                var fsBackup = GetFileSystem(fsOption.MainFileSystem.BackupFileSystem);
                var fs = new BackupFileSystem(fsMain, fsBackup, backgroundTaskQueue);
                return fs;
            }
            else
            {
                var fs = GetFileSystem(fsOption.MainFileSystem.MainFileSystem);
                return fs;
            }
        }

        public IFileSystem GetFileSystem(FileSystemType fsType)
        {
            return _fsDic.ContainsKey(fsType) ? _fsDic[fsType] : null;
        }

        public IFileSystem GetFileSystem(FileSystemSelectOption fileSystemSelectOption)
        {
            if (fileSystemSelectOption.UseMainFileSystem)
            {
                return MainFileSystem;
            }
            else
            {
                return GetFileSystem(fileSystemSelectOption.FileSystemType);
            }
        }
    }
}
