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

[TextJsonConverter(typeof(StringIdTextJsonConverter<UserId>))]
public class RoleId : StringId
{
    public RoleId(string id)
        : base(id)
    {
    }
}

public abstract class UserRole2 : IEquatable<UserRole2>
{
    public RoleId role { get; init; }
    public UserRole2(RoleId role)
    {
        this.role = role;
    }
    public bool Equals(UserRole2 other)
    {
        return role == other.role;
    }
}

public sealed class AdminRole : UserRole2
{
    public AdminRole()
        : base(new RoleId("Admin"))
    {
    }
}