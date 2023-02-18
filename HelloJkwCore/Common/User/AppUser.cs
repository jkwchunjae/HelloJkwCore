namespace Common;

public class AppUser : IEquatable<AppUser>
{
    public static UserId UserId(string loginProvider, string providerKey)
    {
        return new UserId($"{loginProvider}.{providerKey}".ToLower());
    }

    public UserId Id { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime LastLoginTime { get; set; }
    public string UserName { get; set; }
    public string NickName { get; set; }
    [JsonNetIgnore][TextJsonIgnore] public string DisplayName => !string.IsNullOrEmpty(NickName) ? NickName : UserName;
    public string Email { get; set; }
    public ThemeType Theme { get; set; }
    public List<UserRole> Roles { get; set; }

    public AppUser() { }

    public AppUser(string loginProvider, string providerKey)
    {
        Id = UserId(loginProvider, providerKey);
    }

    public bool HasRole(UserRole role)
    {
        return Roles?.Contains(role) ?? false;
    }

    public static bool operator ==(AppUser obj1, AppUser obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }
        if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return obj1.Equals(obj2);
    }

    public static bool operator !=(AppUser obj1, AppUser obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(AppUser other)
    {
        if (ReferenceEquals(other, null))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as AppUser);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}