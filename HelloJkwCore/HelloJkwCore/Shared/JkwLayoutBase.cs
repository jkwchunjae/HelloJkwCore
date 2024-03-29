﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace HelloJkwCore.Shared;

public class JkwLayoutBase : LayoutComponentBase, IDisposable
{
    [Inject] protected IJSRuntime Js { get; set; }
    [Inject] protected IUserStore<AppUser> UserStore { get; set; }
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; }
    protected NavigationManager Navi => NavigationManager;

    [CascadingParameter]
    private AuthenticationState _authenticationState { get; set; }

    protected bool IsAuthenticated { get; private set; }

    protected AppUser User { get; set; }

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

        NavigationManager.LocationChanged += HandleLocationChanged;

        OnPageInitialized();
    }

    protected sealed override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        IsAuthenticated = _authenticationState.User?.Identity?.IsAuthenticated ?? false;

        if (IsAuthenticated)
        {
            var userId = _authenticationState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User = await UserStore.FindByIdAsync(userId, CancellationToken.None);

            if (User == null)
                IsAuthenticated = false; // 가끔 이런 경우가 있나보다.
        }
        else
        {
            User = null;
        }

        await OnPageInitializedAsync();
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

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;

        OnPageDispose();
    }

    private async void HandleLocationChanged(object sender, LocationChangedEventArgs e)
    {
        //if (e.IsNavigationIntercepted == false)
        //    return;

        _authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        IsAuthenticated = _authenticationState.User?.Identity?.IsAuthenticated ?? false;

        if (IsAuthenticated)
        {
            var userId = _authenticationState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User = await UserStore.FindByIdAsync(userId, CancellationToken.None);
        }
        else
        {
            User = null;
        }

        await HandleLocationChanged(e);
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

    protected virtual void OnPageDispose() { }

    protected virtual Task HandleLocationChanged(LocationChangedEventArgs e)
        => Task.CompletedTask;
}