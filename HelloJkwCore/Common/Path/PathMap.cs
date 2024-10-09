namespace Common;

public class PathMap
{
    public Dictionary<string, string>? Dropbox { get; set; }
    public Dictionary<string, string>? Azure { get; set; }
    public Dictionary<string, string>? Local { get; set; }
    public Dictionary<string, string>? InMemory { get; set; }

    public Dictionary<string, string> this[FileSystemType type]
        => type switch
        {
            FileSystemType.Dropbox => Dropbox ?? throw new NotDefinedFileSystemType(type),
            FileSystemType.Azure => Azure ?? throw new NotDefinedFileSystemType(type),
            FileSystemType.Local => Local ?? throw new NotDefinedFileSystemType(type),
            FileSystemType.InMemory => InMemory ?? throw new NotDefinedFileSystemType(type),
            _ => throw new NotUsedFileSystemType(type)
        };
}