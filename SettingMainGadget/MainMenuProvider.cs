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
                new SettingMenu(path: "Setting.Menu.Display.Theme", defaultOrder: 31, type: typeof(Setting.Menu.Display.ThemeGadget)),
                new SettingMenu(path: "Setting.Menu.Sound", defaultOrder: 40, type: typeof(Setting.Menu.SoundGadget)),
                new SettingMenu(path: "Setting.Menu.Sound.SoundMode", defaultOrder: 41, type: typeof(Setting.Menu.Sound.SoundmodeGadget)),
                new SettingMenu(path: "Setting.Menu.Sound.SoundNotification", defaultOrder: 42, type: typeof(Setting.Menu.Sound.SoundnotificationGadget)),
                new SettingMenu(path: "Setting.Menu.Sound.SoundOther", defaultOrder: 43, type: typeof(Setting.Menu.Sound.SoundotherGadget)),
            };
        }
    }
}
