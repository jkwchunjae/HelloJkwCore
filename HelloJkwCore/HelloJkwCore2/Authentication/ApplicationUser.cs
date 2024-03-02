using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore2.Authentication;

public class ApplicationUser : IdentityUser<UserId>
{
    public List<AppLoginInfo> Logins { get; set; } = new();
}
