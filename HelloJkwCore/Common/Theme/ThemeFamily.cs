using JkwExtensions;
using MudBlazor;
using System.Collections.Generic;

namespace Common
{
    public static class ThemeFamily
    {
        private static DefaultDictionary<ThemeType, MudTheme> _dic = new DefaultDictionary<ThemeType, MudTheme>(new Dictionary<ThemeType, MudTheme>()
        {
            [ThemeType.Default] = new MudTheme()
            {
                Palette = new Palette()
                {
                    AppbarBackground = "#222",
                    Black = "#272c34",
                }
            },
            [ThemeType.Dark] = new MudTheme
            {
                Palette = new Palette
                {
                    AppbarBackground = "#222",
                    Black = "#27272f",
                    Background = "#32333d",
                    BackgroundGrey = "#27272f",
                    Surface = "#373740",
                    DrawerBackground = "#27272f",
                    DrawerText = "rgba(255,255,255, 0.50)",
                    DrawerIcon = "rgba(255,255,255, 0.50)",
                    AppbarText = "rgba(255,255,255, 0.70)",
                    Primary = "rgba(255,255,255, 0.70)",
                    TextPrimary = "rgba(255,255,255, 0.70)",
                    TextSecondary = "rgba(255,255,255, 0.50)",
                    ActionDefault = "#adadb1",
                    ActionDisabled = "rgba(255,255,255, 0.26)",
                    ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                    Divider = "rgba(255,255,255, 0.12)",
                    DividerLight = "rgba(255,255,255, 0.06)",
                    TableLines = "rgba(255,255,255, 0.12)",
                    LinesDefault = "rgba(255,255,255, 0.12)",
                    LinesInputs = "rgba(255,255,255, 0.3)",
                    TextDisabled = "rgba(255,255,255, 0.2)",
                }
            }
        }, new MudTheme()
        {
            Palette = new Palette()
            {
                AppbarBackground = "#222",
                Black = "#272c34",
            }
        });

        public static MudTheme GetTheme(ThemeType themeType)
        {
            return _dic[themeType];
        }

        public static ThemeType Next(ThemeType themeType)
        {
            if (themeType == ThemeType.Default)
            {
                return ThemeType.Dark;
            }
            else
            {
                return ThemeType.Default;
            }
        }
    }
}
