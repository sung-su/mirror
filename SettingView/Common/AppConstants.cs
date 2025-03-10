using Tizen.NUI;

namespace SettingView.Common
{
    internal class AppConstants
    {
        private static string LightPlatformThemeId = "org.tizen.default-light-theme";
        private static bool IsLightTheme => ThemeManager.PlatformThemeId == LightPlatformThemeId;

        public static Color BackgroundColor => IsLightTheme ? new Color("#FAFAFA") : new Color("#16131A");
        public static Color TextColor => IsLightTheme ? new Color("#16131A") : new Color("#FAFAFA");
        public static Color ThumbColor => IsLightTheme ? new Color("#FFFEFE") : new Color("#1D1A21");
        public static Shadow ThumbBoxShadow = IsLightTheme ? new Shadow(8.0f, new Color(0.0f, 0.0f, 0.0f, 0.16f), new Vector2(0.0f, 2.0f))
            : new Shadow(8.0f, new Color("#FFFFFF29"), new Vector2(0.0f, 1.0f));

        private static Size displaySize = NUIApplication.GetScreenSize();
        public static double ScreenWidthRatio = displaySize.Width / 1920;
        public static double ScreenHeightRatio = displaySize.Height / 1024;
    }
}
