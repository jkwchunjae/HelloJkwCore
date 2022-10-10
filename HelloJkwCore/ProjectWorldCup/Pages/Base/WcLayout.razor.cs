using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace ProjectWorldCup.Pages.Base;

public partial class WcLayout : LayoutComponentBase
{
    [Inject] protected IJSRuntime Js { get; set; }
    [Inject] protected IUserStore<AppUser> UserStore { get; set; }
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; }
    [CascadingParameter] private AuthenticationState _authenticationState { get; set; }
    protected bool IsAuthenticated { get; private set; }
    protected AppUser User { get; set; }

    bool _drawerOpen = true;
    MudTheme currentTheme = ThemeFamily.GetTheme(ThemeType.Default);
    ThemeType _currentThemeType = ThemeType.Default;


    protected sealed override void OnInitialized()
    {
        base.OnInitialized();

        NavigationManager.LocationChanged += HandleLocationChanged;
    }
    protected override async Task OnInitializedAsync()
    {
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

        _currentThemeType = ThemeType.Default;
        if (IsAuthenticated)
        {
            _currentThemeType = User.Theme;
            currentTheme = ThemeFamily.GetTheme(User.Theme);
        }
    }
    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
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
    }

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    void ToggleTheme()
    {
        _currentThemeType = ThemeFamily.Next(_currentThemeType);
        currentTheme = ThemeFamily.GetTheme(_currentThemeType);

        if (IsAuthenticated)
        {
            User.Theme = _currentThemeType;
            UserStore.UpdateAsync(User, CancellationToken.None);
        }
    }
}
