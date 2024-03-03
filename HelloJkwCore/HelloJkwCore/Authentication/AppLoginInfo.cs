using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore.Authentication;

public class AppLoginInfo
{
    public required string Provider { get; set; }
    public required string ProviderKey { get; set; }
    public string? ProviderDisplayName { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime LastLoginTime { get; set; }
    public UserId? ConnectedUserId { get; set; }

    [JsonIgnore] public UserLoginInfo LoginInfo => new UserLoginInfo(Provider, ProviderKey, ProviderDisplayName);
}
