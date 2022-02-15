namespace Common;

public class NotUsedFileSystemType : Exception
{
    public NotUsedFileSystemType(FileSystemType type)
        : base($"Not used file system: {type}")
    {
    }
}

public class NotDefinedFileSystemType : Exception
{
    public NotDefinedFileSystemType(FileSystemType type)
        : base($"Not defined file system: {type}")
    {
    }
}

public class NotDefinedPath : Exception
{
    public NotDefinedPath(FileSystemType type, string path)
        : base($"Not defined path: {type} {path}")
    {
    }
}
