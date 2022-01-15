namespace Common;

public class Paths
{
    private readonly Dictionary<string, string> _pathDic;

    public Paths(PathMap option, FileSystemType fsType)
    {
        _pathDic = option.Default.Select(x => x.Key)
            .Concat(option[fsType].Select(x => x.Key))
            .Distinct()
            .Select(key => new
            {
                Key = key,
                Value = option[fsType].ContainsKey(key) ? option[fsType][key] : option.Default[key],
            })
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public string this[string key]
    {
        get => _pathDic[key];
    }
}