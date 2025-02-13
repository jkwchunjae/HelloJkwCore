﻿namespace HelloJkwCore.Authentication;

internal sealed class IdentityUserAccessor(AppUserManager userManager, IdentityRedirectManager redirectManager)
{
    public async Task<AppUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}