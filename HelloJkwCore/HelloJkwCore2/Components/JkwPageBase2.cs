using HelloJkwCore2.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace HelloJkwCore2.Components;

public class JkwPageBase2 : ComponentBase
{
    [Inject] protected IJSRuntime Js { get; set; } = null!;
    [Inject] private AppUserManager UserManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    protected bool IsAuthenticated { get; private set; } = false;
    protected ApplicationUser? User { get; private set; } = default;

    protected sealed override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var contextUser = authState?.User;
        var authenticated = false;
        if (contextUser != null)
        {
            var user = await UserManager.GetUserAsync(contextUser);
            if (user != null)
            {
                authenticated = true;
                IsAuthenticated = true;
                User = user;
            }
        }

        if (!authenticated)
        {
            IsAuthenticated = false;
            User = null;
        }
    }
}