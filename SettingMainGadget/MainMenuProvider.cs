using SettingCore;

namespace SettingMainGadget
{
    public class MainMenuProvider : SettingMenuProvider
    {
        public override SettingMenu[] Provide()
        {
            return new SettingMenu[]
            {
                new SettingMenu(path: "Setting.Menu.Wifi", defaultOrder: 10, type: typeof(Setting.Menu.WifiGadget)),
                new SettingMenu(path: "Setting.Menu.Bluetooth", defaultOrder: 20, type: typeof(Setting.Menu.BluetoothGadget)),
                new SettingMenu(path: "Setting.Menu.Display", defaultOrder: 30, type: typeof(Setting.Menu.DisplayGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Font", defaultOrder: 31, type: typeof(Setting.Menu.Display.DisplayFontGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Font.FontSize", defaultOrder: 32, type: typeof(Setting.Menu.Display.DisplayFontSizeGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Font.FontType", defaultOrder: 33, type: typeof(Setting.Menu.Display.DisplayFontTypeGadget)),
                new SettingMenu(path: "Setting.Menu.Display.TimeOut", defaultOrder: 34, type: typeof(Setting.Menu.Display.DisplayTimeOutGadget)),
                new SettingMenu(path: "Setting.Menu.Display.Theme", defaultOrder: 35, type: typeof(Setting.Menu.Display.DisplayThemeGadget)),
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
            };
        }
    }
}
