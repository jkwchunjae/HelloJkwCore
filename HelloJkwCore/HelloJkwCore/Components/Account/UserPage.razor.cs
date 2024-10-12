using HelloJkwCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace HelloJkwCore.Components.Account;

public partial class UserPage : JkwPageBase
{
    public const string LinkLoginCallbackAction = "LinkLoginCallback";
    [Inject] private SignInManager<AppUser> SignInManager { get; set; } = null!;
    [Inject] private AppUserManager UserManager { get; set; } = default!;
    [Inject] private IdentityRedirectManager RedirectManager { get; set; } = default!;

    [CascadingParameter] private HttpContext? HttpContext { get; set; }
    [SupplyParameterFromForm] private string? LoginProvider { get; set; }
    [SupplyParameterFromForm] private string? ProviderKey { get; set; }
    [SupplyParameterFromQuery] private string? Action { get; set; }

    private AuthenticationScheme[] externalLogins = [];

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private TvOptions? tvOptionsLogins;

    protected override async Task OnPageInitializedAsync()
    {
        tvOptionsLogins = MakeTvOptionsLogins();
        if (HttpMethods.IsGet(HttpContext?.Request?.Method ?? string.Empty) && Action == LinkLoginCallbackAction)
        {
            await OnGetLinkLoginCallbackAsync();
        }
        else
        {
            externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
        }
    }

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var hasAntiforgeryToken = await Js.InvokeAsync<bool>("checkAntiforgeryToken");
            if (!hasAntiforgeryToken)
            {
                await Js.InvokeVoidAsync("reload");
                return;
            }
        }
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

    private async Task OnGetLinkLoginCallbackAsync()
    {
        if (User == null)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The user is not authenticated.", HttpContext!);
        }

        var info = await SignInManager.GetExternalLoginInfoAsync(User.Id.Id);
        if (info is null)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: Could not load external login info.", HttpContext!);
        }

        var result = await UserManager.AddLoginAsync(User, info);
        if (!result.Succeeded)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The external login was not added. External logins can only be associated with one account.", HttpContext!);
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext!.SignOutAsync(IdentityConstants.ExternalScheme);

        RedirectManager.RedirectToCurrentPageWithStatus("The external login was added.", HttpContext!);
    }

    private async Task RemoveLoginFromUserAsync(string loginProvider, string providerKey)
    {
        var result = await UserManager.RemoveLoginAsync(User!, loginProvider, providerKey);
        if (!result.Succeeded)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The external login was not removed.", HttpContext!);
        }

        await Js.InvokeVoidAsync("reload");
    }

    private TvOptions MakeTvOptionsLogins()
    {
        return new TvOptions
        {
            Title = "유저 정보",
            GlobalOpenDepth = 2,
            Style =
            {
                RootClass = { "mt-8" },
            },
            Buttons =
            {
                new TvPopupAction<AppLoginInfo>
                {
                    Action = (loginInfo) => RemoveLoginFromUserAsync(loginInfo.Provider, loginInfo.ProviderKey),
                    Label = "연결 해제",
                    PopupContent = (loginInfo) => $"정말로 {loginInfo.Provider} 계정을 연결 해제하시겠습니까?",
                    PopupTitle = (loginInfo) => $"{loginInfo.Provider} 계정 연결 해제",
                    Condition = (loginInfo, depth) => User!.Logins.Count() > 1,
                    InnerButtonOptions =
                    {
                        ConfirmLabel = "연결 해제",
                        ConfirmButtonStyle =
                        {
                            Color = Color.Error,
                        },
                        CloseLabel = "취소",
                    },
                    Style =
                    {
                        Variant = Variant.Outlined,
                        Color = Color.Error,
                    },
                },
            },
            TitleButtons =
            {
                new TvAction<AppUser>
                {
                    Label = "로그아웃",
                    Action = async user =>
                    {
                        await Js.InvokeVoidAsync("logout");
                    },
                },
                new TvAction<AppUser>
                {
                    Label = "새로고침",
                    Action = async (user) =>
                    {
                        await Js.InvokeVoidAsync("reload");
                    },
                },
            },
            DisableKeys =
            {
                nameof(AppUser.NormalizedUserName),
                nameof(AppUser.Email),
                nameof(AppUser.EmailConfirmed),
                nameof(AppUser.NormalizedEmail),
                nameof(AppUser.PasswordHash),
                nameof(AppUser.SecurityStamp),
                nameof(AppUser.ConcurrencyStamp),
                nameof(AppUser.PhoneNumber),
                nameof(AppUser.PhoneNumberConfirmed),
                nameof(AppUser.TwoFactorEnabled),
                nameof(AppUser.LockoutEnd),
                nameof(AppUser.LockoutEnabled),
                nameof(AppUser.AccessFailedCount),
                nameof(AppLoginInfo.Provider),
                nameof(AppLoginInfo.ProviderKey),
                nameof(AppLoginInfo.ProviderDisplayName),
                nameof(AppLoginInfo.CreateTime),
                nameof(AppLoginInfo.ConnectedUserId),
                nameof(AppLoginInfo.LoginInfo),
            },
            DateTime = new TvDateTimeOption
            {
                Format = "yyyy-MM-dd HH:mm:ss",
            },
        };
    }
}