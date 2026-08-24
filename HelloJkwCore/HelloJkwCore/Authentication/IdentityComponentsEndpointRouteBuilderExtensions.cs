using HelloJkwCore.Components.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace HelloJkwCore.Authentication;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        if (endpoints.ServiceProvider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            accountGroup.MapPost("/DevelopmentLogin", async (
                HttpContext context,
                [FromServices] UserManager<AppUser> userManager,
                [FromServices] SignInManager<AppUser> signInManager,
                [FromServices] ILoggerFactory loggerFactory,
                [FromForm] string userId,
                [FromForm] string? returnUrl) =>
            {
                var localReturnUrl = GetLocalReturnUrl(returnUrl);
                var user = string.IsNullOrWhiteSpace(userId)
                    ? null
                    : await userManager.FindByIdAsync(userId.Trim());

                if (user is null)
                {
                    return TypedResults.LocalRedirect(BuildDevelopmentLoginErrorUrl(
                        context,
                        localReturnUrl,
                        "해당 ID의 사용자를 찾을 수 없습니다."));
                }

                await signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: "Development");
                loggerFactory
                    .CreateLogger("DevelopmentLogin")
                    .LogInformation("User {UserId} signed in using the development login.", user.Id);

                return TypedResults.LocalRedirect(localReturnUrl);
            });

            accountGroup.MapPost("/DevelopmentCreateUser", async (
                HttpContext context,
                [FromServices] UserManager<AppUser> userManager,
                [FromServices] SignInManager<AppUser> signInManager,
                [FromServices] ILoggerFactory loggerFactory,
                [FromForm] string userId,
                [FromForm] string? accountType,
                [FromForm] string? returnUrl) =>
            {
                var localReturnUrl = GetLocalReturnUrl(returnUrl);
                var normalizedUserId = userId.Trim();
                var validationError = ValidateDevelopmentUserId(normalizedUserId);

                if (validationError is not null)
                {
                    return TypedResults.LocalRedirect(BuildDevelopmentLoginErrorUrl(
                        context,
                        localReturnUrl,
                        validationError));
                }

                if (await userManager.FindByIdAsync(normalizedUserId) is not null)
                {
                    return TypedResults.LocalRedirect(BuildDevelopmentLoginErrorUrl(
                        context,
                        localReturnUrl,
                        "이미 존재하는 사용자 ID입니다. 기존 사용자 로그인 기능을 사용하세요."));
                }

                var user = new AppUser
                {
                    Id = new UserId(normalizedUserId),
                    UserName = normalizedUserId,
                    NickName = normalizedUserId,
                };

                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errorMessage = string.Join(" ", createResult.Errors.Select(error => error.Description));
                    return TypedResults.LocalRedirect(BuildDevelopmentLoginErrorUrl(
                        context,
                        localReturnUrl,
                        $"사용자를 만들 수 없습니다. {errorMessage}"));
                }

                var isAdmin = string.Equals(accountType, nameof(UserRole.Admin), StringComparison.OrdinalIgnoreCase);
                if (isAdmin)
                {
                    var roleResult = await userManager.AddToRoleAsync(user, nameof(UserRole.Admin));
                    if (!roleResult.Succeeded)
                    {
                        await userManager.DeleteAsync(user);
                        var errorMessage = string.Join(" ", roleResult.Errors.Select(error => error.Description));
                        return TypedResults.LocalRedirect(BuildDevelopmentLoginErrorUrl(
                            context,
                            localReturnUrl,
                            $"Admin 권한을 부여할 수 없습니다. {errorMessage}"));
                    }
                }

                await signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: "Development");
                loggerFactory
                    .CreateLogger("DevelopmentLogin")
                    .LogInformation(
                        "User {UserId} was created with account type {AccountType} using the development login.",
                        user.Id,
                        isAdmin ? nameof(UserRole.Admin) : "User");

                return TypedResults.LocalRedirect(localReturnUrl);
            });
        }

        accountGroup.MapPost("/PerformExternalLogin", (
            HttpContext context,
            [FromServices] SignInManager<AppUser> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            IEnumerable<KeyValuePair<string, StringValues>> query = [
                new("ReturnUrl", returnUrl),
                //new("Action", ExternalLogin.LoginCallbackAction)];
                new("Action", "LoginCallback")];

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/ExternalLogin",
                QueryString.Create(query));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        accountGroup.MapGet("/PerformExternalLogin", (
            HttpContext context,
            [FromServices] SignInManager<AppUser> signInManager,
            [FromQuery] string provider,
            [FromQuery] string returnUrl) =>
        {
            IEnumerable<KeyValuePair<string, StringValues>> query = [
                new("ReturnUrl", returnUrl),
                //new("Action", ExternalLogin.LoginCallbackAction)];
                new("Action", "LoginCallback")];

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/ExternalLogin",
                QueryString.Create(query));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            SignInManager<AppUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        manageGroup.MapPost("/LinkExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<AppUser> signInManager,
            [FromForm] string provider) =>
        {
            // Clear the existing external cookie to ensure a clean login process
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/Manage/ExternalLogins",
                QueryString.Create("Action", UserPage.LinkLoginCallbackAction));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
            return TypedResults.Challenge(properties, [provider]);
        });

        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        return accountGroup;
    }

    private static string GetLocalReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return "/";
        }

        if (returnUrl[0] == '/')
        {
            return returnUrl.Length == 1 || (returnUrl[1] != '/' && returnUrl[1] != '\\')
                ? returnUrl
                : "/";
        }

        if (returnUrl[0] == '~' && returnUrl.Length > 1 && returnUrl[1] == '/')
        {
            return returnUrl.Length == 2 || (returnUrl[2] != '/' && returnUrl[2] != '\\')
                ? returnUrl
                : "/";
        }

        return "/";
    }

    private static string? ValidateDevelopmentUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return "사용자 ID를 입력하세요.";
        }

        if (userId.Length > 128)
        {
            return "사용자 ID는 128자 이하여야 합니다.";
        }

        if (Path.GetInvalidFileNameChars().Any(userId.Contains))
        {
            return "사용자 ID에 파일 이름으로 사용할 수 없는 문자가 포함되어 있습니다.";
        }

        return null;
    }

    private static string BuildDevelopmentLoginErrorUrl(HttpContext context, string returnUrl, string errorMessage)
    {
        IEnumerable<KeyValuePair<string, StringValues>> query = [
            new("ReturnUrl", returnUrl),
            new("DevelopmentLoginError", errorMessage)
        ];

        return UriHelper.BuildRelative(
            context.Request.PathBase,
            "/login",
            QueryString.Create(query));
    }
}
