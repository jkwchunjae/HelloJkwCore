namespace Common;

public interface IFileSystemService
{
    IFileSystem GetFileSystem(FileSystemSelectOption fileSystemSelectOption, PathMap pathOption);
}

public class FileSystemService : IFileSystemService
{
    private readonly FileSystemOption _fsOption;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<FileSystemService> _logger;
    private IBackgroundTaskQueue _queue;
    private ISerializer _serializer;

    public FileSystemService(
        FileSystemOption fsOption,
        IBackgroundTaskQueue backgroundTaskQueue,
        ISerializer serializer,
        ILoggerFactory loggerFactory)
    {
        _fsOption = fsOption;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FileSystemService>();
        _queue = backgroundTaskQueue;
        _serializer = serializer;
    }

    private Dictionary<FileSystemType, IFileSystem> CreateFileSystem(FileSystemOption fsOption, PathMap pathOption)
    {
        Dictionary<FileSystemType, IFileSystem> fsDic = new();

        if (pathOption.Dropbox != null)
        {
            if (fsOption.Dropbox != null)
            {
                var dropboxClient = DropboxExtensions.GetDropboxClient(fsOption.Dropbox);
                fsDic.Add(FileSystemType.Dropbox, new DropboxFileSystem(pathOption, dropboxClient, _serializer));
            }
        }
        if (pathOption.Azure != null)
        {
            if (fsOption.Azure != null)
            {
                fsDic.Add(FileSystemType.Azure, new AzureFileSystem(pathOption, fsOption.Azure.ConnectionString, _loggerFactory));
            }
        }
        if (pathOption.InMemory != null)
        {
            fsDic.Add(FileSystemType.InMemory, new InMemoryFileSystem(pathOption));
        }
        if (pathOption.Local != null)
        {
            fsDic.Add(FileSystemType.Local, new LocalFileSystem(pathOption));
        }

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
        if (fsDic.TryGetValue(fsType, out var fs))
        {
            return fs;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public static IFileSystem CreateMainFileSystem(this Dictionary<FileSystemType, IFileSystem> fsDic, FileSystemOption fsOption, IBackgroundTaskQueue backgroundTaskQueue)
    {
        if (fsOption.MainFileSystem?.UseBackup ?? false)
        {
            var fsMain = fsDic.GetFileSystem(fsOption.MainFileSystem.MainFileSystem);
            var fsBackup = fsDic.GetFileSystem(fsOption.MainFileSystem.BackupFileSystem);
            var fs = new BackupFileSystem(fsMain, fsBackup, backgroundTaskQueue);
            return fs;
        }
        else
        {
            var fs = fsDic.GetFileSystem(fsOption.MainFileSystem?.MainFileSystem ?? FileSystemType.Local);
            return fs;
        }
    }
}