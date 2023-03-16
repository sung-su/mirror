using SettingCore;

namespace SettingMainGadget
{
    public class MainMenuProvider : SettingMenuProvider
    {
        public static string Display = "Display";
        public static string Display_Brightness = "Display.Brightness";
        public static string Display_Font = "Display.Font";
        public static string Display_FontSize = "Display.Font.FontSize";
        public static string Display_FontType = "Display.Font.FontType";
        public static string Display_Timeout = "Display.Timeout";
        public static string Display_Theme = "Display.Theme";

        public override SettingMenu[] Provide()
        {
            return new SettingMenu[]
            {
                new SettingMenu(path: Display, defaultOrder: 30, type: typeof(Setting.Menu.DisplayGadget)),
                new SettingMenu(path: Display_Brightness, defaultOrder: 31),
                new SettingMenu(path: Display_Font, defaultOrder: 32, type: typeof(Setting.Menu.Display.DisplayFontGadget)),
                new SettingMenu(path: Display_FontSize, defaultOrder: 33, type: typeof(Setting.Menu.Display.DisplayFontSizeGadget)),
                new SettingMenu(path: Display_FontType, defaultOrder: 34, type: typeof(Setting.Menu.Display.DisplayFontTypeGadget)),
                new SettingMenu(path: Display_Timeout, defaultOrder: 35, type: typeof(Setting.Menu.Display.DisplayTimeOutGadget)),
                new SettingMenu(path: Display_Theme, defaultOrder: 36, type: typeof(Setting.Menu.Display.DisplayThemeGadget)),
                new SettingMenu(path: "Sound", defaultOrder: 40, type: typeof(Setting.Menu.SoundGadget)),
                new SettingMenu(path: "Sound.SoundMode", defaultOrder: 41, type: typeof(Setting.Menu.Sound.SoundmodeGadget)),
                new SettingMenu(path: "Sound.SoundNotification", defaultOrder: 42, type: typeof(Setting.Menu.Sound.SoundnotificationGadget)),
                new SettingMenu(path: "Sound.SoundOther", defaultOrder: 43, type: typeof(Setting.Menu.Sound.SoundotherGadget)),
                new SettingMenu(path: "Sound.Media", defaultOrder: 44),
                new SettingMenu(path: "Sound.Notification", defaultOrder: 45),
                new SettingMenu(path: "Sound.System", defaultOrder: 46),
                new SettingMenu(path: "DateTime", defaultOrder: 50, type: typeof(Setting.Menu.DateTimeGadget)),
                new SettingMenu(path: "DateTime.SetDate", defaultOrder: 51, type: typeof(Setting.Menu.DateTime.DateTimeSetDateGadget)),
                new SettingMenu(path: "DateTime.SetTime", defaultOrder: 52, type: typeof(Setting.Menu.DateTime.DateTimeSetTimeGadget)),
                new SettingMenu(path: "DateTime.SetTimezone", defaultOrder: 53, type: typeof(Setting.Menu.DateTime.DateTimeSetTimezoneGadget)),
                new SettingMenu(path: "LanguageInput", defaultOrder: 60, type: typeof(Setting.Menu.LanguageInputGadget)),
                new SettingMenu(path: "LanguageInput.DisplayLanguage", defaultOrder: 61, type: typeof(Setting.Menu.LanguageInput.LanguageInputDisplayLanguageGadget)),
                new SettingMenu(path: "About", defaultOrder: 70, type: typeof(Setting.Menu.AboutGadget)),
                new SettingMenu(path: "About.ManageCertificates", defaultOrder: 71),
                new SettingMenu(path: "About.OpenSourceLicenses", defaultOrder: 72, type: typeof(Setting.Menu.AboutLegalInfoGadget)),
                new SettingMenu(path: "About.DeviceInfo", defaultOrder: 73),
                new SettingMenu(path: "About.RenameDevice", defaultOrder: 74),
                new SettingMenu(path: "About.ModelNumber", defaultOrder: 75),
                new SettingMenu(path: "About.TizenVersion", defaultOrder: 76),
                new SettingMenu(path: "About.Cpu", defaultOrder: 77),
                new SettingMenu(path: "About.Ram", defaultOrder: 78),
                new SettingMenu(path: "About.Resolution", defaultOrder: 79),
                new SettingMenu(path: "About.DeviceStatus", defaultOrder: 80, type: typeof(Setting.Menu.AboutDeviceStatusGadget)),
            };
        }
    }
}
