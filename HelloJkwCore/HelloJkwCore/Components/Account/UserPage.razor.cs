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

    protected override async Task OnPageInitializedAsync()
    {
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

    private async Task OnSubmitAsync()
    {
        var result = await UserManager.RemoveLoginAsync(User!, LoginProvider!, ProviderKey!);
        if (!result.Succeeded)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The external login was not removed.", HttpContext!);
        }

        await SignInManager.RefreshSignInAsync(User!);
        RedirectManager.RedirectToCurrentPageWithStatus("The external login was removed.", HttpContext!);
    }
}