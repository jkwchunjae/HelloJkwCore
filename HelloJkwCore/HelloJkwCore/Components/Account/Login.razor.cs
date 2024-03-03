using HelloJkwCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MudBlazor;

namespace HelloJkwCore.Components.Account;

public partial class Login : ComponentBase
{
    [Inject] public IJSRuntime Js { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = default!;

    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }
    private AuthenticationScheme[] externalLogins = [];
    private string UserAgent = string.Empty;
    private bool IsInAppBrowser => ExceptApps.Any(app => UserAgent.Contains(app));
    private string[] ExceptApps = new[] { "kakao", "naver" };

    protected override async Task OnInitializedAsync()
    {
        externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var userAgent = await Js.InvokeAsync<string>("getUserAgent");
            await Js.InvokeVoidAsync("console.log", "UserAgent", userAgent);
            UserAgent = userAgent.ToLower();
            StateHasChanged();
        }
    }

    private void LoginGoogleInOtherBrowser()
    {
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add("구글 로그인은 크롬에서 해주세요.");
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
