namespace Common;

public class AppUser
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
    [JsonIgnore] public string DisplayName => !string.IsNullOrEmpty(NickName) ? NickName : UserName;
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
}