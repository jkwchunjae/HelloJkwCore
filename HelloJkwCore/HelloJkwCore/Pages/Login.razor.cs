using MudBlazor;

namespace HelloJkwCore.Pages;

public partial class Login : ComponentBase
{
    [Inject] public IJSRuntime Js { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }
    [Parameter, SupplyParameterFromQuery] public string ReturnUrl { get; set; }

    private bool IsChromeBrowser => ExceptApps.Any(app => UserAgent.Contains(app)) ? false : true;
    private string[] ExceptApps = new[] { "kakao", "naver" };
    private string UserAgent = string.Empty;

    private string GoogleLoginUri => string.IsNullOrEmpty(ReturnUrl)
        ? $"/LoginExternal?provider=Google"
        : $"/LoginExternal?provider=Google&returnUrl={ReturnUrl}";
    private string KakaoLoginUri => string.IsNullOrEmpty(ReturnUrl)
        ? $"/LoginExternal?provider=KakaoTalk"
        : $"/LoginExternal?provider=KakaoTalk&returnUrl={ReturnUrl}";

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
}