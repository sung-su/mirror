using Tizen.Network.Connection;
using Tizen.NUI;

namespace SettingMainGadget.Common
{
    public static class AppAttributes
    {
        public static Window DefaultWindow { get => NUIApplication.GetDefaultWindow(); }

        public static Size2D WindowSize { get => DefaultWindow.Size; }
        public static int WindowWidth { get => WindowSize.Width; }
        public static int WindowHeight { get => WindowSize.Height; }

        public static bool IsPortrait { get => WindowWidth < WindowHeight; }

        public static bool IsLightTheme { get => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";  }

        public static readonly Size2D PopupActionButtonSize = new(252, 48);
        public static readonly Vector4 PopupActionButtonCornerRadius = 24.SpToPx();
        public static readonly Extents PopupActionButtonMargin = new Extents(4, 4, 4, 4).SpToPx();

        public static readonly Shadow PopupBoxShadow = IsLightTheme ? new Shadow(8.0f, new Color(0.0f, 0.0f, 0.0f, 0.16f), new Vector2(0.0f, 2.0f)) : new Shadow(6.0f, new Color("#FFFFFF29"), new Vector2(0.0f, 1.0f));

    }
}
