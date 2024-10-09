
using HelloJkwCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore.Components.Account;

public partial class LinkLogin : JkwPageBase
{
    public const string LinkLoginCallbackAction = "LinkLoginCallback";

    [Inject] private SignInManager<AppUser> SignInManager { get; set; } = default!;
    [Inject] private AppUserManager UserManager { get; set; } = default!;
    [Inject] private IdentityRedirectManager RedirectManager { get; set; } = default!;

    [CascadingParameter] private HttpContext HttpContext { get; set; } = default!;
    [SupplyParameterFromQuery] private string? Action { get; set; }

    protected override async Task OnPageInitializedAsync()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method) && Action == LinkLoginCallbackAction)
        {
            await OnGetLinkLoginCallbackAsync();
        }
    }

    private async Task OnGetLinkLoginCallbackAsync()
    {
        if (User == null)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The user is not authenticated.", HttpContext);
        }

        var info = await SignInManager.GetExternalLoginInfoAsync(User.Id.Id);
        if (info is null)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: Could not load external login info.", HttpContext);
        }

        var result = await UserManager.AddLoginAsync(User, info);
        if (!result.Succeeded)
        {
            RedirectManager.RedirectToCurrentPageWithStatus("Error: The external login was not added. External logins can only be associated with one account.", HttpContext);
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        RedirectManager.RedirectToCurrentPageWithStatus("The external login was added.", HttpContext);
    }
}