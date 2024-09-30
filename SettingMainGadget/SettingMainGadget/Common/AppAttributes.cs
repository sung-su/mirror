using Tizen.Network.Connection;
using Tizen.NUI;

namespace SettingMainGadget.Common
{
    public static class AppAttributes
    {
        public static readonly Window DefaultWindow = NUIApplication.GetDefaultWindow();

        public static readonly Size2D WindowSize = DefaultWindow.Size;
        public static readonly int WindowWidth = WindowSize.Width;
        public static readonly int WindowHeight = WindowSize.Height;

        public static readonly bool IsPortrait = WindowWidth < WindowHeight;

        public static readonly bool IsLightTheme = ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        public static readonly Size2D PopupActionButtonSize = new (252, 48);
        public static readonly Vector4 PopupActionButtonCornerRadius = 24.SpToPx();
        public static readonly Shadow PopupBoxShadow = IsLightTheme ? new Shadow(8.0f, new Color(0.0f, 0.0f, 0.0f, 0.16f), new Vector2(0.0f, 2.0f)) : new Shadow(6.0f, new Color("#FFFFFF29"), new Vector2(0.0f, 1.0f));
    }
}
