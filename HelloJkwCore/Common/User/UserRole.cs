namespace Common;

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum UserRole
{
    Admin,
    DiaryWriter,
    BadukCreator,
    JangTak9,
}
