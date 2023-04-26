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
        public static string Sound = "Sound";
        public static string Sound_Mode = "Sound.Mode";
        public static string Sound_Notification = "Sound.Notification";
        public static string Sound_Other = "Sound.Other";
        public static string Sound_MediaSlider = "Sound.MediaSlider";
        public static string Sound_NotificationSlider = "Sound.NotificationSlider";
        public static string Sound_SystemSlider = "Sound.SystemSlider";
        public static string DateTime = "DateTime";
        public static string DateTime_AutoUpdate = "DateTime.AutoUpdate";
        public static string DateTime_SetDate = "DateTime.SetDate";
        public static string DateTime_SetTime = "DateTime.SetTime";
        public static string DateTime_SetTimezone = "DateTime.SetTimezone";
        public static string DateTime_TimeFormat = "DateTime.TimeFormat";
        public static string Language = "Language";
        public static string Language_Display = "Language.DisplayLanguage";
        public static string Language_KeyboardHeader = "Language.KeyboardHeader";
        public static string Language_InputAssistanceHeader = "Language.InputAssistanceHeader";
        public static string Language_BodySpeech = "Language.BodySpeech";
        public static string About = "About";
        public static string About_ManageCertificates = "About.ManageCertificates";
        public static string About_OpenSourceLicenses = "About.OpenSourceLicenses";
        public static string About_ScalableUI = "About.ScalableUI";
        public static string About_DeviceInfo = "About.DeviceInfo";
        public static string About_RenameDevice = "About.RenameDevice";
        public static string About_ModelNumber = "About.ModelNumber";
        public static string About_TizenVersion = "About.TizenVersion";
        public static string About_Cpu = "About.Cpu";
        public static string About_Ram = "About.Ram";
        public static string About_Resolution = "About.Resolution";
        public static string About_DeviceStatus = "About.DeviceStatus";
        public static string About_DeviceStatus_bt_address = "About.DeviceStatus.BluetoothAddress";
        public static string About_DeviceStatus_wifi_mac_address = "About.DeviceStatus.WifiMacAddress";
        public static string About_DeviceStatus_storage = "About.DeviceStatus.Storage";
        public static string About_DeviceStatus_cpu_usage = "About.DeviceStatus.CpuUsage";

        //storage
        public static string Storage = "Storage";
        public static string Storage_InternalUsage = "Storage.InternalUsage";
        public static string Storage_Used = "Storage.Used";
        public static string Storage_TotalInternal = "Storage.TotalInternal";
        public static string Storage_FreeInternal = "Storage.FreeInternal";
        public static string Storage_UsageSummary = "Storage.UsageSummary";
        public static string Storage_UsageIndicator = "Storage.UsageIndicator";
        public static string Storage_ExternalUsage = "Storage.ExternalUsage";
        public static string Storage_ExternalStorage = "Storage.ExternalStorage";
        public static string Storage_DefaultSettings = "Storage.DefaultSettings";
        public static string Storage_Apps = "Storage.Apps";

        //external
        public static string Language_InputMethod = "Language.InputMethod";
        public static string Language_AutoFill = "Language.AutoFill";
        public static string Language_VoiceControl = "Language.VoiceControl";
        public static string Language_TTS = "Language.TTS";
        public static string Language_STT = "Language.STT";

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
                new SettingMenu(path: Sound, defaultOrder: 40, type: typeof(Setting.Menu.SoundGadget)),
                new SettingMenu(path: Sound_Mode, defaultOrder: 41, type: typeof(Setting.Menu.Sound.SoundmodeGadget)),
                new SettingMenu(path: Sound_Notification, defaultOrder: 42, type: typeof(Setting.Menu.Sound.SoundnotificationGadget)),
                new SettingMenu(path: Sound_Other, defaultOrder: 43, type: typeof(Setting.Menu.Sound.SoundotherGadget)),
                new SettingMenu(path: Sound_MediaSlider, defaultOrder: 44),
                new SettingMenu(path: Sound_NotificationSlider, defaultOrder: 45),
                new SettingMenu(path: Sound_SystemSlider, defaultOrder: 46),
                new SettingMenu(path: DateTime, defaultOrder: 50, type: typeof(Setting.Menu.DateTimeGadget)),
                new SettingMenu(path: DateTime_AutoUpdate, defaultOrder: 51),
                new SettingMenu(path: DateTime_SetDate, defaultOrder: 52, type: typeof(Setting.Menu.DateTime.DateTimeSetDateGadget)),
                new SettingMenu(path: DateTime_SetTime, defaultOrder: 53, type: typeof(Setting.Menu.DateTime.DateTimeSetTimeGadget)),
                new SettingMenu(path: DateTime_SetTimezone, defaultOrder: 54, type: typeof(Setting.Menu.DateTime.DateTimeSetTimezoneGadget)),
                new SettingMenu(path: DateTime_TimeFormat, defaultOrder: 55),
                new SettingMenu(path: Language, defaultOrder: 60, type: typeof(Setting.Menu.LanguageInputGadget)),
                new SettingMenu(path: Language_Display, defaultOrder: 61, type: typeof(Setting.Menu.LanguageInput.LanguageInputDisplayLanguageGadget)),
                new SettingMenu(path: Language_KeyboardHeader, defaultOrder: 62),
                new SettingMenu(path: Language_InputMethod, defaultOrder: 63),
                new SettingMenu(path: Language_InputAssistanceHeader, defaultOrder: 64),
                new SettingMenu(path: Language_AutoFill, defaultOrder: 65),
                new SettingMenu(path: Language_BodySpeech, defaultOrder: 66),
                new SettingMenu(path: Language_VoiceControl, defaultOrder: 67),
                new SettingMenu(path: Language_TTS, defaultOrder: 68),
                new SettingMenu(path: Language_STT, defaultOrder: 69),
                new SettingMenu(path: About, defaultOrder: 70, type: typeof(Setting.Menu.AboutGadget)),
                new SettingMenu(path: About_ManageCertificates, defaultOrder: 10),
                new SettingMenu(path: About_OpenSourceLicenses, defaultOrder: 20, type: typeof(Setting.Menu.AboutLegalInfoGadget)),
                new SettingMenu(path: About_ScalableUI, defaultOrder: -30, type: typeof(SettingMainGadget.About.AboutScalableGadget)),
                new SettingMenu(path: About_DeviceInfo, defaultOrder: 40),
                new SettingMenu(path: About_RenameDevice, defaultOrder: 50),
                new SettingMenu(path: About_ModelNumber, defaultOrder: 60),
                new SettingMenu(path: About_TizenVersion, defaultOrder: 70),
                new SettingMenu(path: About_Cpu, defaultOrder: 80),
                new SettingMenu(path: About_Ram, defaultOrder: 90),
                new SettingMenu(path: About_Resolution, defaultOrder: 100),
                new SettingMenu(path: About_DeviceStatus, defaultOrder: 110, type: typeof(Setting.Menu.AboutDeviceStatusGadget)),
                new SettingMenu(path: About_DeviceStatus_bt_address, defaultOrder: 10),
                new SettingMenu(path: About_DeviceStatus_wifi_mac_address, defaultOrder: 20),
                new SettingMenu(path: About_DeviceStatus_storage, defaultOrder: 30),
                new SettingMenu(path: About_DeviceStatus_cpu_usage, defaultOrder: 40),
                //storage
                new SettingMenu(path: Storage, defaultOrder: 120, type: typeof(Setting.Menu.StorageGadget)),
                new SettingMenu(path: Storage_InternalUsage, defaultOrder: 10),
                new SettingMenu(path: Storage_Used, defaultOrder: 15),
                new SettingMenu(path: Storage_UsageIndicator, defaultOrder: 20),
                new SettingMenu(path: Storage_TotalInternal, defaultOrder: 25),
                new SettingMenu(path: Storage_FreeInternal, defaultOrder: 30),
                new SettingMenu(path: Storage_UsageSummary, defaultOrder: 35),
                new SettingMenu(path: Storage_ExternalUsage, defaultOrder: 40),
                new SettingMenu(path: Storage_ExternalStorage, defaultOrder: 45),
                new SettingMenu(path: Storage_DefaultSettings, defaultOrder: 50, typeof(Setting.Menu.Storage.DefaultStorageGadget)),
                new SettingMenu(path: Storage_Apps, defaultOrder: 60, typeof(Setting.Menu.Storage.AppsStorageGadget)),
            };
        }
    }
}
