using HelloJkwCore2.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore2.Components.Account;

public partial class Login : ComponentBase
{
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = null!;

    private AuthenticationScheme[] externalLogins = [];

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
    }

    private string LoginImage(string provider)
    {
        return provider switch
        {
            "Google" => "/images/login/btn_google_signin_dark_normal_web.png",
            "KakaoTalk" => "/images/login/kakao_login_medium_narrow.png",
            _ => string.Empty,
        };
    }
}
