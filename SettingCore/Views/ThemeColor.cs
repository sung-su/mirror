using Tizen.NUI;

namespace SettingCore.Views
{
    public class ThemeColor
    {
        private bool isLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        public Color Normal => isLightTheme ? lightThemeNormalColor : darkThemeNormalColor;
        public Color Selected => isLightTheme ? lightThemeSelectedColor : darkThemeSelectedColor;
        public Color Disabled => isLightTheme ? lightThemeDisabledColor : darkThemeDisabledColor;

        private Color lightThemeNormalColor = Color.Transparent;
        private Color darkThemeNormalColor = Color.Transparent;

        private Color lightThemeSelectedColor = Color.Transparent;
        private Color darkThemeSelectedColor = Color.Transparent;

        private Color lightThemeDisabledColor = Color.Transparent;
        private Color darkThemeDisabledColor = Color.Transparent;

        public ThemeColor(Color lightNormal, Color darkNormal, Color lightSelected, Color darkSelected)
        {
            lightThemeNormalColor = lightNormal;
            darkThemeNormalColor = darkNormal;

            lightThemeSelectedColor = lightSelected;
            darkThemeSelectedColor = darkSelected;
        }

        public ThemeColor(Color lightNormal, Color darkNormal, Color lightSelected, Color darkSelected, Color lightDisabled, Color darkDisabled)
        {
            lightThemeNormalColor = lightNormal;
            darkThemeNormalColor = darkNormal;

            lightThemeSelectedColor = lightSelected;
            darkThemeSelectedColor = darkSelected;

            lightThemeDisabledColor = lightDisabled;
            darkThemeDisabledColor = darkDisabled;
        }
    }
}
