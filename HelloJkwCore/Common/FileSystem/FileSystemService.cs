using Common.Dropbox;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Common;

public interface IFileSystemService
{
    //IFileSystem MainFileSystem { get; }
    //IFileSystem GetFileSystem(FileSystemType fsType);
    IFileSystem GetFileSystem(FileSystemSelectOption fileSystemSelectOption, PathMap pathOption);
}

public class FileSystemService : IFileSystemService
{
    //private readonly Dictionary<FileSystemType, IFileSystem> _fsDic = new();
    private readonly FileSystemOption _fsOption;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<FileSystemService> _logger;
    private IBackgroundTaskQueue _queue;

    //public IFileSystem MainFileSystem { get; private set; }

    public FileSystemService(
        FileSystemOption fsOption,
        IBackgroundTaskQueue backgroundTaskQueue,
        ILoggerFactory loggerFactory)
    {
        _fsOption = fsOption;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<FileSystemService>();
        _queue = backgroundTaskQueue;
    }

    private Dictionary<FileSystemType, IFileSystem> CreateFileSystem(FileSystemOption fsOption, PathMap pathOption)
    {
        Dictionary<FileSystemType, IFileSystem> fsDic = new();

        if (fsOption.Dropbox != null)
        {
            var dropboxClient = DropboxHelper.GetDropboxClient(fsOption.Dropbox);
            fsDic.Add(FileSystemType.Dropbox, new DropboxFileSystem(pathOption, dropboxClient));
        }
        if (fsOption.Azure != null)
        {
            fsDic.Add(FileSystemType.Azure, new AzureFileSystem(pathOption, fsOption.Azure.ConnectionString, _loggerFactory));
        }
        fsDic.Add(FileSystemType.InMemory, new InMemoryFileSystem(pathOption));
        fsDic.Add(FileSystemType.Local, new LocalFileSystem(pathOption));

        return fsDic;
    }

    public IFileSystem GetFileSystem(FileSystemSelectOption fileSystemSelectOption, PathMap pathOption)
    {
        var fsDic = CreateFileSystem(_fsOption, pathOption);

        if (fileSystemSelectOption.UseMainFileSystem)
        {
            return fsDic.CreateMainFileSystem(_fsOption, _queue);
        }
        else
        {
            return fsDic.GetFileSystem(fileSystemSelectOption.FileSystemType);
        }
    }
}

static class FsDicExtension
{
    public static IFileSystem GetFileSystem(this Dictionary<FileSystemType, IFileSystem> fsDic, FileSystemType fsType)
    {
        return fsDic.ContainsKey(fsType) ? fsDic[fsType] : null;
    }

    public static IFileSystem CreateMainFileSystem(this Dictionary<FileSystemType, IFileSystem> fsDic, FileSystemOption fsOption, IBackgroundTaskQueue backgroundTaskQueue)
    {
        if (fsOption.MainFileSystem.UseBackup)
        {
            var fsMain = fsDic.GetFileSystem(fsOption.MainFileSystem.MainFileSystem);
            var fsBackup = fsDic.GetFileSystem(fsOption.MainFileSystem.BackupFileSystem);
            var fs = new BackupFileSystem(fsMain, fsBackup, backgroundTaskQueue);
            return fs;
        }
        else
        {
            var fs = fsDic.GetFileSystem(fsOption.MainFileSystem.MainFileSystem);
            return fs;
        }
    }
}