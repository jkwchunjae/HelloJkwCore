namespace Common;

public class Paths
{
    private FileSystemType _fileSystemType { get; init; }
    private Dictionary<string, string> _pathDic { get; init; }

    public Paths(PathMap option, FileSystemType fsType)
    {
        _fileSystemType = fsType;
        _pathDic = option[fsType];
    }

    public string this[string key]
    {
        get
        {
            if (_pathDic.ContainsKey(key))
            {
                return _pathDic[key];
            }
            else
            {
                throw new NotDefinedPath(_fileSystemType, key);
            }
        }
    }
}