namespace Common;

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum FileSystemType
{
    InMemory,
    Local,
    Dropbox,
    Azure,
}