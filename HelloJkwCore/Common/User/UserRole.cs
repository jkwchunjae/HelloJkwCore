namespace Common;

[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
public enum UserRole
{
    Admin,
    DiaryWriter,
    SuFcAdmin,
    BadukCreator,
}