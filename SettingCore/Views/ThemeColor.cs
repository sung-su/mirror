using Tizen.NUI;

namespace SettingCore.Views
{
    public class ThemeColor
    {
        private bool isLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        public Color Normal => isLightTheme ? lightThemeNormalColor : darkThemeNormalColor;
        public Color Selected => isLightTheme ? lightThemeSelectedColor : darkThemeSelectedColor;

        private Color lightThemeNormalColor = Color.Transparent;
        private Color darkThemeNormalColor = Color.Transparent;

        private Color lightThemeSelectedColor = Color.Transparent;
        private Color darkThemeSelectedColor = Color.Transparent;

        public void SetNormalColor(Color lightThemeColor, Color darkThemeColor)
        {
            lightThemeNormalColor = lightThemeColor;
            darkThemeNormalColor = darkThemeColor;
        }

        public void SetSelectedColor(Color lightThemeColor, Color darkThemeColor)
        {
            lightThemeSelectedColor = lightThemeColor;
            darkThemeSelectedColor = darkThemeColor;
        }
    }
}
