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

        await OnPageInitializedAsync();
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