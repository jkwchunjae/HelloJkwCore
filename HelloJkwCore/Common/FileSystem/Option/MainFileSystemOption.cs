namespace Common;

public class MainFileSystemOption
{
    public FileSystemType MainFileSystem { get; set; }
    public FileSystemType BackupFileSystem { get; set; }
    public bool UseBackup { get; set; }
}