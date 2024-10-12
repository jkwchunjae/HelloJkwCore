namespace Common;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
[TvString]
public enum UserRole
{
    Admin,
    DiaryWriter,
    BadukCreator,
    JangTak9,
}
