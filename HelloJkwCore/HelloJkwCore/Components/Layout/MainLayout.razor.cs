using Common;
using HelloJkwCore.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HelloJkwCore.Components.Layout;

public partial class MainLayout : JkwLayoutBase
{
    bool _drawerOpen = true;
    bool _isDarkMode = false;
    MudTheme currentTheme = ThemeFamily.GetTheme(ThemeType.Default);
    ThemeType _currentThemeType = ThemeType.Default;

    protected override Task OnPageInitializedAsync()
    {
        _isDarkMode = false;
        if (IsAuthenticated)
        {
            _currentThemeType = User!.Theme;
            _isDarkMode = _currentThemeType == ThemeType.Dark;
            currentTheme = ThemeFamily.GetTheme(User.Theme);
        }

        return Task.CompletedTask;
    }

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    async Task ToggleTheme()
    {
        _currentThemeType = ThemeFamily.Next(_currentThemeType);
        _isDarkMode = _currentThemeType == ThemeType.Dark;
        currentTheme = ThemeFamily.GetTheme(_currentThemeType);

        if (IsAuthenticated)
        {
           User!.Theme = _currentThemeType;
           await UserStore.UpdateAsync(User, CancellationToken.None);
        }
    }
}
