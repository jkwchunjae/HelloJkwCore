namespace Common;

public class PathMap
{
    public Dictionary<string, string> Dropbox { get; set; }
    public Dictionary<string, string> Azure { get; set; }
    public Dictionary<string, string> Local { get; set; }
    public Dictionary<string, string> InMemory { get; set; }

    public Dictionary<string, string> this[FileSystemType type]
    {
        get
        {
            switch (type)
            {
                case FileSystemType.Dropbox:
                    if (Dropbox == null)
                        throw new NotDefinedFileSystemType(type);
                    return Dropbox;
                case FileSystemType.Azure:
                    if (Azure == null)
                        throw new NotDefinedFileSystemType(type);
                    return Azure;
                case FileSystemType.Local:
                    if (Local == null)
                        throw new NotDefinedFileSystemType(type);
                    return Local;
                case FileSystemType.InMemory:
                    if (InMemory == null)
                        throw new NotDefinedFileSystemType(type);
                    return InMemory;
                default:
                    throw new NotUsedFileSystemType(type);
            }
        }
    }
}