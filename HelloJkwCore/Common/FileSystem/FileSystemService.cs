namespace Common;

public interface IFileSystemService
{
    IFileSystem GetFileSystem(FileSystemSelectOption fileSystemSelectOption, PathMap pathOption);
}

public class FileSystemService : IFileSystemService
{
    private readonly FileSystemOption _fsOption;
    private readonly IEnumerable<IFileSystemBuilder> _fileSystems;
    private IBackgroundTaskQueue _queue;

    public FileSystemService(
        FileSystemOption fsOption,
        IEnumerable<IFileSystemBuilder> fileSystems,
        IBackgroundTaskQueue backgroundTaskQueue
    )
    {
        _fsOption = fsOption;
        _fileSystems = fileSystems;
        _queue = backgroundTaskQueue;
    }

    public IFileSystem GetFileSystem(FileSystemSelectOption fileSystemSelectOption, PathMap pathMap)
    {
        if (fileSystemSelectOption.UseMainFileSystem)
        {
            return CreateMainFileSystem(_fsOption, pathMap, _queue);
        }
        else
        {
            return GetFileSystemBuilder(fileSystemSelectOption.FileSystemType)
                .Build(pathMap);
        }
    }

    private IFileSystemBuilder GetFileSystemBuilder(FileSystemType fsType)
    {
        var fileSystem = _fileSystems.FirstOrDefault(fs => fs.FileSystemType == fsType);
        if (fileSystem == null)
        {
            throw new ArgumentException();
        }
        return fileSystem;
    }

    private IFileSystem CreateMainFileSystem(FileSystemOption fsOption, PathMap pathMap, IBackgroundTaskQueue backgroundTaskQueue)
    {
        if (fsOption.MainFileSystem?.UseBackup ?? false)
        {
            var fsMain = GetFileSystemBuilder(fsOption.MainFileSystem.MainFileSystem)
                .Build(pathMap);
            var fsBackup = GetFileSystemBuilder(fsOption.MainFileSystem.BackupFileSystem)
                .Build(pathMap);
            var fs = new BackupFileSystem(fsMain, fsBackup, backgroundTaskQueue);
            return fs;
        }
        else
        {
            var fs = GetFileSystemBuilder(fsOption.MainFileSystem?.MainFileSystem ?? FileSystemType.Local)
                .Build(pathMap);
            return fs;
        }
    }
}
