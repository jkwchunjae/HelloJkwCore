using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using System.Threading;
using MudBlazor;

namespace HelloJkwCore.Shared
{
    public partial class MainLayout : JkwLayoutBase
    {
        bool _drawerOpen = true;
        MudTheme currentTheme = ThemeFamily.GetTheme(ThemeType.Default);
        ThemeType _currentThemeType = ThemeType.Default;

        protected override Task OnPageInitializedAsync()
        {
            _currentThemeType = ThemeType.Default;
            if (IsAuthenticated)
            {
                _currentThemeType = User.Theme;
                currentTheme = ThemeFamily.GetTheme(User.Theme);
            }

            return Task.CompletedTask;
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
}
