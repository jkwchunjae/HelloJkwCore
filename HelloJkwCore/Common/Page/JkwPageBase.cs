using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Common;

public class JkwPageBase : ComponentBase
{
    [Inject] protected IJSRuntime Js { get; set; } = null!;
    [Inject] private AppUserManager UserManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    protected NavigationManager Navi => NavigationManager;

    protected bool IsAuthenticated { get; private set; } = false;
    protected AppUser? User { get; private set; } = default;

    protected sealed override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var (authenticated, user) = await TryGetAppUser();

        if (authenticated)
        {
            IsAuthenticated = true;
            User = user;

            await UpdateLastLoginTime(User!);
        }
        else
        {
            IsAuthenticated = false;
            User = null;
        }

        await OnPageInitializedAsync();

        async Task<(bool Authenticated, AppUser? User)> TryGetAppUser()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var contextUser = authState?.User;

            if (contextUser != null)
            {
                var user = await UserManager.GetUserAsync(contextUser);
                var authenticated = user != null;
                return (authenticated, user);
            }
            else
            {
                return (false, default);
            }
        }

        async Task UpdateLastLoginTime(AppUser user)
        {
            if (DateTime.Now - user.LastLoginTime > TimeSpan.FromDays(1))
            {
                user.LastLoginTime = DateTime.Now;
                await UserManager.UpdateAsync(user);
            }
        }
    }

    public sealed override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        await SetPageParametersAsync(parameters);
    }

    protected sealed override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        OnPageAfterRender(firstRender);
    }

    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await OnPageAfterRenderAsync(firstRender);
    }

    protected sealed override void OnInitialized()
    {
        base.OnInitialized();
        OnPageInitialized();
    }

    protected sealed override void OnParametersSet()
    {
        base.OnParametersSet();
        OnPageParametersSet();
    }

    protected sealed override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await OnPageParametersSetAsync();
    }

    public virtual Task SetPageParametersAsync(ParameterView parameters)
        => Task.CompletedTask;

    protected virtual void OnPageAfterRender(bool firstRender) { }

    protected virtual Task OnPageAfterRenderAsync(bool firstRender)
        => Task.CompletedTask;

    protected virtual void OnPageInitialized() { }

    protected virtual Task OnPageInitializedAsync()
        => Task.CompletedTask;

    protected virtual void OnPageParametersSet() { }

    protected virtual Task OnPageParametersSetAsync()
        => Task.CompletedTask;
}