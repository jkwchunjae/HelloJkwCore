
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HelloJkwCore2.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore2.Components.Account;

public partial class ExternalLogin : ComponentBase
{
    [Inject] SignInManager<ApplicationUser> SignInManager { get; set; } = null!;
    [Inject] UserManager<ApplicationUser> UserManager { get; set; } = null!;
    [Inject] IUserStore<ApplicationUser> UserStore { get; set; } = null!;
    [Inject] NavigationManager NavigationManager { get; set; } = null!;
    [Inject] IdentityRedirectManager RedirectManager { get; set; } = null!;
    [Inject] ILogger<ExternalLogin> Logger { get; set; } = null!;

    [CascadingParameter] private HttpContext HttpContext { get; set; } = default!;
    [SupplyParameterFromForm] private InputModel Input { get; set; } = new();
    [SupplyParameterFromQuery] private string? RemoteError { get; set; }
    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }
    [SupplyParameterFromQuery] private string? Action { get; set; }

    public const string LoginCallbackAction = "LoginCallback";
    private string? message;
    private ExternalLoginInfo externalLoginInfo = default!;

    private string? ProviderDisplayName => externalLoginInfo.ProviderDisplayName;


    protected override async Task OnInitializedAsync()
    {
        if (RemoteError is not null)
        {
            RedirectManager.RedirectToWithStatus("Account/Login", $"Error from external provider: {RemoteError}", HttpContext);
        }

        var info = await SignInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            RedirectManager.RedirectToWithStatus("Account/Login", "Error loading external login information.", HttpContext);
        }

        externalLoginInfo = info;

        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            if (Action == LoginCallbackAction)
            {
                await OnLoginCallbackAsync();
                return;
            }

            // We should only reach this page via the login callback, so redirect back to
            // the login page if we get here some other way.
            RedirectManager.RedirectTo("Account/Login");
        }
    }

    private async Task OnLoginCallbackAsync()
    {
        // Sign in the user with this external login provider if the user already has a login.
        var result = await SignInManager.ExternalLoginSignInAsync(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            Logger.LogInformation(
                "{Name} logged in with {LoginProvider} provider.",
                externalLoginInfo.Principal.Identity?.Name,
                externalLoginInfo.LoginProvider);
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.IsLockedOut)
        {
            RedirectManager.RedirectTo("Account/Lockout");
        }

        // If the user does not have an account, then ask the user to create an account.
        if (externalLoginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
        {
            Input.Name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        }

        await OnValidSubmitAsync();
    }

    private async Task OnValidSubmitAsync()
    {
        var user = CreateUser();

        await UserStore.SetUserNameAsync(user, Input.Name, CancellationToken.None);

        var result = await UserManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await UserManager.AddLoginAsync(user, externalLoginInfo);
            if (result.Succeeded)
            {
                Logger.LogInformation("User created an account using {Name} provider.", externalLoginInfo.LoginProvider);

                await SignInManager.SignInAsync(user, isPersistent: false, externalLoginInfo.LoginProvider);
                RedirectManager.RedirectTo(ReturnUrl);
            }
        }

        message = $"Error: {string.Join(",", result.Errors.Select(error => error.Description))}";
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return new ApplicationUser
            {
                Id = new UserId($"user.{externalLoginInfo.ProviderKey}"),
            };
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor");
        }
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}