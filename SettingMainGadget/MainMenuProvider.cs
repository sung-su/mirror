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
        public static string Language_BodySpeach = "Language.BodySpeach";
        public static string About = "About";
        public static string About_ManageCertificates = "About.ManageCertificates";
        public static string About_OpenSourceLicenses = "About.OpenSourceLicenses";
        public static string About_DeviceInfo = "About.DeviceInfo";
        public static string About_RenameDevice = "About.RenameDevice";
        public static string About_ModelNumber = "About.ModelNumber";
        public static string About_TizenVersion = "About.TizenVersion";
        public static string About_Cpu = "About.Cpu";
        public static string About_Ram = "About.Ram";
        public static string About_Resolution = "About.Resolution";
        public static string About_DeviceStatus = "About.DeviceStatus";

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
                new SettingMenu(path: Language_BodySpeach, defaultOrder: 66),
                new SettingMenu(path: Language_VoiceControl, defaultOrder: 67),
                new SettingMenu(path: Language_TTS, defaultOrder: 68),
                new SettingMenu(path: Language_STT, defaultOrder: 69),
                new SettingMenu(path: About, defaultOrder: 70, type: typeof(Setting.Menu.AboutGadget)),
                new SettingMenu(path: About_ManageCertificates, defaultOrder: 71),
                new SettingMenu(path: About_OpenSourceLicenses, defaultOrder: 72, type: typeof(Setting.Menu.AboutLegalInfoGadget)),
                new SettingMenu(path: About_DeviceInfo, defaultOrder: 73),
                new SettingMenu(path: About_RenameDevice, defaultOrder: 74),
                new SettingMenu(path: About_ModelNumber, defaultOrder: 75),
                new SettingMenu(path: About_TizenVersion, defaultOrder: 76),
                new SettingMenu(path: About_Cpu, defaultOrder: 77),
                new SettingMenu(path: About_Ram, defaultOrder: 78),
                new SettingMenu(path: About_Resolution, defaultOrder: 79),
                new SettingMenu(path: About_DeviceStatus, defaultOrder: 80, type: typeof(Setting.Menu.AboutDeviceStatusGadget)),
            };
        }
    }
}
