using SettingMainGadget.TextResources;
using SettingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.System;
using Tizen.NUI;

namespace SettingMainGadget.LanguageInput
{
    public static class LanguageInputDisplayLanguageManager
    {
        private const string VconfRegionAutomatic = "db/setting/region_automatic";
        private const string VconfLanguageAutomatic = "db/setting/lang_automatic";

        private const string VconfWidgetLanguage = "db/menu_widget/language";

        public class DisplayLanguageInfo
        {
            private readonly string Locale = null;
            private readonly string Name = null;
            private readonly string Language = null;
            private readonly string Mcc = null;

            public DisplayLanguageInfo(string locale, string name, string language, string mcc)
            {
                Locale = locale;
                Name = name;
                Language = language;
                Mcc = mcc;
            }

            public string GetLocale() => Locale;

            public string GetName() => Name;

            public string GetLanguage() => Language;

            public string GetMcc() => Mcc;
        }

        public static readonly List<DisplayLanguageInfo> LanguageList = new List<DisplayLanguageInfo>()
        {
            new DisplayLanguageInfo("en_US", "English (United States)", "English(US)", "310,311,313,316"),
            new DisplayLanguageInfo("ko_KR", "한국어", "Korean", "450")
        };

        public static string GetDisplayLanguage()
        {
            if (!Tizen.Vconf.TryGetString(VconfWidgetLanguage, out string locale))
            {
                Logger.Warn($"could not get value for {VconfWidgetLanguage}");
            }

            String[] qStrings = locale.Split('.');
            locale = qStrings[0];

            return locale;
        }

        public static int GetDisplayLanguageIndex()
        {
            string curlanguage = GetDisplayLanguage();

            var languageinfo = LanguageList.Where(x => x.GetLocale() == curlanguage).FirstOrDefault();

            if(languageinfo != null)
            {
                return LanguageList.IndexOf(languageinfo);
            }

            return -1;
        }

        public static string GetDisplayLanguageName(NUIGadget gadget)
        {
            string title = "N/A";

            if (!Tizen.Vconf.TryGetBool(VconfLanguageAutomatic, out bool lang_automatic))
            {
                Logger.Warn($"could not get value for {VconfLanguageAutomatic}");
            }

            if (lang_automatic)
            {
                return gadget.NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ANSWERINGMODE_AUTOMATIC));
            }

            string locale = GetDisplayLanguage();

            var displayLanguage = LanguageList.Where(x => x.GetLocale() == locale).FirstOrDefault();

            if (displayLanguage != null)
            {
                return displayLanguage.GetName();
            }

            return title;
        }

        public static void SetDisplayLanguage(string locale)
        {
            /* [control] set automatic - TRUE */
            Tizen.Vconf.SetBool("db/setting/lang_automatic", false);

            /* [control] set vconf language */
            // system_settings_set_value_string(SYSTEM_SETTINGS_KEY_LOCALE_LANGUAGE, pnode->locale);
            SystemSettings.LocaleLanguage = locale;
            // TODO : NUI setting corresponding to elm_language_set(temp);

            if (!Tizen.Vconf.TryGetBool(VconfRegionAutomatic, out bool region_automatic))
            {
                Logger.Warn($"could not get value for {VconfRegionAutomatic}");
            }

            if (region_automatic)
            {
                /* [control] region format set - if 'automatic region' */
                SystemSettings.LocaleCountry = locale;
            }

            /* 2. GET SELECTED LANG */
            if (!Tizen.Vconf.TryGetString(VconfWidgetLanguage, out string paLanguage))
            {
                Logger.Warn($"could not get value for {VconfWidgetLanguage}");
            }

            String[] qStrings = paLanguage.Split('.');
            string qString = qStrings[0];
            Logger.Debug($"language: {paLanguage}, {qString}");

            /* 3. SET DEFAULT */
            /* default UI language */
            // i18n_ulocale_set_default(qString);
            // To-do : NUI setting corresponding to i18n_ulocale_set_default();

            SystemSettings.LocaleCountry = paLanguage;
        }
    }
}
