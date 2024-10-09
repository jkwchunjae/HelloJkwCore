using Microsoft.AspNetCore.Identity;

namespace Common;

public class AppUser : IdentityUser<UserId>, IEquatable<AppUser>
{
    public string? NickName { get; set; }
    public List<AppLoginInfo> Logins { get; set; } = new();
    public List<UserRole> Roles { get; set; } = new();
    public DateTime CreateTime { get; set; }
    public DateTime LastLoginTime { get; set; }
    public ThemeType Theme { get; set; }

    [TextJsonIgnore] public string? DisplayName => NickName ?? UserName;

    public bool HasRole(UserRole role) => Roles.Contains(role);

    public static bool operator ==(AppUser? obj1, AppUser? obj2)
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

    public static bool operator !=(AppUser? obj1, AppUser? obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(AppUser? other)
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

    public override bool Equals(object? obj)
    {
        if (obj is AppUser user)
        {
            return Equals(user);
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
