using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore.Authentication;

public class ApplicationUser : IdentityUser<UserId>
{
    public List<AppLoginInfo> Logins { get; set; } = new();
    public List<UserRole> Roles { get; set; } = new();
    public DateTime CreateTime { get; set; }
    public DateTime LastLoginTime { get; set; }

    public string? Nickname { get; set; }
    [JsonIgnore] public string? DisplayName => Nickname ?? UserName;

    public bool HasRole(UserRole role) => Roles.Contains(role);
}
