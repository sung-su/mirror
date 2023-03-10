using SettingCore;

namespace SettingMainGadget
{
    public class MainMenuProvider : SettingMenuProvider
    {
        public override SettingMenu[] Provide()
        {
            return new SettingMenu[]
            {
                new SettingMenu(path: "Setting.Menu.Display", defaultOrder: 30, type: typeof(Setting.Menu.DisplayGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Brightness", defaultOrder: 31),
                new SettingMenu(path: "Setting.Menu.Display.Font", defaultOrder: 32, type: typeof(Setting.Menu.Display.DisplayFontGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Font.FontSize", defaultOrder: 33, type: typeof(Setting.Menu.Display.DisplayFontSizeGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Font.FontType", defaultOrder: 34, type: typeof(Setting.Menu.Display.DisplayFontTypeGadget)),
                new SettingMenu(path: "Setting.Menu.Display.TimeOut", defaultOrder: 35, type: typeof(Setting.Menu.Display.DisplayTimeOutGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Theme", defaultOrder: 36, type: typeof(Setting.Menu.Display.DisplayThemeGadget)),
                new SettingMenu(path: "Setting.Menu.Sound", defaultOrder: 40, type: typeof(Setting.Menu.SoundGadget)),
                new SettingMenu(path: "Setting.Menu.Sound.SoundMode", defaultOrder: 41, type: typeof(Setting.Menu.Sound.SoundmodeGadget)),
                new SettingMenu(path: "Setting.Menu.Sound.SoundNotification", defaultOrder: 42, type: typeof(Setting.Menu.Sound.SoundnotificationGadget)),
                new SettingMenu(path: "Setting.Menu.Sound.SoundOther", defaultOrder: 43, type: typeof(Setting.Menu.Sound.SoundotherGadget)),
                new SettingMenu(path: "Setting.Menu.Sound.Media", defaultOrder: 44),
                new SettingMenu(path: "Setting.Menu.Sound.Notification", defaultOrder: 45),
                new SettingMenu(path: "Setting.Menu.Sound.System", defaultOrder: 46),
                new SettingMenu(path: "Setting.Menu.DateTime", defaultOrder: 50, type: typeof(Setting.Menu.DateTimeGadget)),
                new SettingMenu(path: "Setting.Menu.DateTime.SetDate", defaultOrder: 51, type: typeof(Setting.Menu.DateTime.DateTimeSetDateGadget)),
                new SettingMenu(path: "Setting.Menu.DateTime.SetTime", defaultOrder: 52, type: typeof(Setting.Menu.DateTime.DateTimeSetTimeGadget)),
                new SettingMenu(path: "Setting.Menu.DateTime.SetTimezone", defaultOrder: 53, type: typeof(Setting.Menu.DateTime.DateTimeSetTimezoneGadget)),
                new SettingMenu(path: "Setting.Menu.LanguageInput", defaultOrder: 60, type: typeof(Setting.Menu.LanguageInputGadget)),
                new SettingMenu(path: "Setting.Menu.LanguageInput.DisplayLanguage", defaultOrder: 61, type: typeof(Setting.Menu.LanguageInput.LanguageInputDisplayLanguageGadget)),
                new SettingMenu(path: "Setting.Menu.About", defaultOrder: 70, type: typeof(Setting.Menu.AboutGadget)),
                new SettingMenu(path: "Setting.Menu.About.ManageCertificates", defaultOrder: 71),
                new SettingMenu(path: "Setting.Menu.About.OpenSourceLicenses", defaultOrder: 72, type: typeof(Setting.Menu.AboutLegalInfoGadget)),
                new SettingMenu(path: "Setting.Menu.About.DeviceInfo", defaultOrder: 73),
                new SettingMenu(path: "Setting.Menu.About.RenameDevice", defaultOrder: 74),
                new SettingMenu(path: "Setting.Menu.About.ModelNumber", defaultOrder: 75),
                new SettingMenu(path: "Setting.Menu.About.TizenVersion", defaultOrder: 76),
                new SettingMenu(path: "Setting.Menu.About.Cpu", defaultOrder: 77),
                new SettingMenu(path: "Setting.Menu.About.Ram", defaultOrder: 78),
                new SettingMenu(path: "Setting.Menu.About.Resolution", defaultOrder: 79),
                new SettingMenu(path: "Setting.Menu.About.DeviceStatus", defaultOrder: 80, type: typeof(Setting.Menu.AboutDeviceStatusGadget)),
            };
        }
    }
}
