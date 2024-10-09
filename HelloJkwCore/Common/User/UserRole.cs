namespace Common;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum UserRole
{
    Admin,
    DiaryWriter,
    BadukCreator,
    JangTak9,
}
