namespace Common;

[JsonConverter(typeof(StringEnumConverter))]
public enum FileSystemType
{
    InMemory,
    Local,
    Dropbox,
    Azure,
}