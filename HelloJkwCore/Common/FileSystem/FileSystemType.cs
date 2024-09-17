namespace Common;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum FileSystemType
{
    InMemory,
    Local,
    Dropbox,
    Azure,
}