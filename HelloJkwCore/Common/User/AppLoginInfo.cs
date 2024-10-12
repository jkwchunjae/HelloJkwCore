using Microsoft.AspNetCore.Identity;

namespace Common;

public class AppLoginInfo
{
    public required string Provider { get; set; }
    public required string ProviderKey { get; set; }
    public string? ProviderDisplayName { get; set; }
    public DateTime CreateTime { get; set; }
    public UserId? ConnectedUserId { get; set; }

    [TextJsonIgnore] public UserLoginInfo LoginInfo => new UserLoginInfo(Provider, ProviderKey, ProviderDisplayName);
    [TextJsonIgnore] public string 계정종류 => Provider;
    [TextJsonIgnore] public string 아이디 => ProviderKey;
    [TextJsonIgnore] public DateTime 연결시간 => CreateTime;
}
