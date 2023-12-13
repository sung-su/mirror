using Tizen.NUI;

namespace SettingCore
{
    public static class Tools
    {
        public static bool IsLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";
    }
}
