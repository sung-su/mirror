using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using System.Collections.ObjectModel;
using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
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


        public string GetLocale()
        {
            return Locale;
        }
        public string GetName()
        {
            return Name;
        }

        public string GetLanguage()
        {
            return Language;
        }

        public string GetMcc()
        {
            return Mcc;
        }
    };


    class SettingContent_DisplayLanguage : SettingContent_Base
    {
        private static readonly DisplayLanguageInfo[] LanguageList =
        {
            new DisplayLanguageInfo("en_US", "English (United States)", "English(US)", "310,311,313,316"),
            new DisplayLanguageInfo("ko_KR", "한국어", "Korean", "450")
        };

        private string[] PickerItems;
        public SettingContent_DisplayLanguage()
            : base()
        {


            mTitle = Resources.IDS_ST_BUTTON_BACK;

            PickerItems = new string[LanguageList.Length];
            for (int i = 0; i < LanguageList.Length; i++)
            {
                PickerItems[i] = LanguageList[i].GetName();
            }
        }

        protected override View CreateContent(Window window)
        {
            var picker = new Picker()
            {
                //WidthSpecification = LayoutParamPolicies.MatchParent,
                //HeightSpecification = LayoutParamPolicies.MatchParent,
                //                Size = new Size(100, 200),
            };

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(PickerItems);
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = PickerItems.Length - 1;
            Tizen.Log.Debug("NUI", "DisplayedValues : " + picker.DisplayedValues);

            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {

                Tizen.Log.Debug("NUI", String.Format("current : {0}", PickerItems[picker.CurrentValue]));

                SetDisplayLanguage(picker.CurrentValue);

                RequestWidgetPop();
            };


            var content = new View()
            {

                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };
            content.Add(new TextLabel(Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE));
            content.Add(picker);
            content.Add(button);

            return content;
        }

        void SetDisplayLanguage(int index)
        {
            /* [control] set automatic - TRUE */
            Vconf.SetBool("db/setting/lang_automatic", false);

            string locale = LanguageList[index].GetLocale();
            

            /* [control] set vconf language */
            // system_settings_set_value_string(SYSTEM_SETTINGS_KEY_LOCALE_LANGUAGE, pnode->locale);
            //system_settings_set_value_string(SYSTEM_SETTINGS_KEY_LOCALE_LANGUAGE, pnode->locale);
            SystemSettings.LocaleLanguage = locale;
            //elm_language_set(temp);
            string temp = locale + ".UTF-8";
            // To-do : NUI setting corresponding to elm_language_set(temp);

            bool region_automatic = Vconf.GetBool("db/setting/region_automatic");
            if (region_automatic)
            {
                /* [control] region format set - if 'automatic region' */
                SystemSettings.LocaleCountry = locale;

                SetDateFormat(locale);
            }


            /* 2. GET SELECTED LANG */
            String paLanguage = Vconf.GetString("db/menu_widget/language");
            String[] qStrings = paLanguage.Split('.');
            string qString = qStrings[0];
            Tizen.Log.Debug("NUI", String.Format("{0} -> {1}", paLanguage, qString));


            /* 3. SET DEFAULT */
            /* default UI language */
            // i18n_ulocale_set_default(qString);
            // To-do : NUI setting corresponding to i18n_ulocale_set_default();

            SetDateFormat(qString);


            paLanguage = Vconf.GetString("db/menu_widget/language");
            SystemSettings.LocaleCountry = paLanguage;

        }



        private static void SetDateFormat(string region)
        {
#if false
            SettingLaIData *ad = (SettingLaIData *)data;
            char *ret_str = NULL;
            i18n_uchar *uret = NULL;
            i18n_uchar customSkeleton[SETTING_STR_SLP_LEN] = { 0, };
            i18n_uchar bestPattern[SETTING_STR_SLP_LEN] = { 0, };
            char bestPatternString[SETTING_STR_SLP_LEN] = { 0, };
            char *skeleton = DATA_FORMAT_SKELETON;
            int i = 0;
            int j = 0;
            int32_t bestPatternCapacity;
            int len;
            char region_format[4] = { 0, };
            int ymd[3] = { 0, };
            char *date_format_str[DATA_FORMAT_CATEGORY_NUM] = {"dMy","Mdy","yMd","ydM" };
            int err = SETTING_RETURN_SUCCESS;

            uret = i18n_ustring_copy_ua_n(customSkeleton, skeleton, safeStrLen(skeleton));
            setting_retvm_if(!uret, SETTING_RETURN_FAIL, "i18n_ustring_copy_ua_n is fail");

            if (ad->pattern_generator == NULL || safeStrCmp(region, ad->prev_locale) != 0) {
                if (ad->pattern_generator != NULL) {
                    i18n_udatepg_destroy(ad->pattern_generator);
                    ad->pattern_generator = NULL;
                }

                i18n_udatepg_create(region, ad->pattern_generator);
            }

            bestPatternCapacity = (int32_t) (sizeof(bestPattern) / sizeof((bestPattern)[0]));
            int32_t best_pattern_len;
            (void)i18n_udatepg_get_best_pattern(ad->pattern_generator, customSkeleton, i18n_ustring_get_length(customSkeleton), bestPattern, bestPatternCapacity, &best_pattern_len);

            ret_str = i18n_ustring_copy_au(bestPatternString, bestPattern);
            setting_retvm_if(!ret_str, SETTING_RETURN_FAIL, "i18n_ustring_copy_au is fail");

            len = safeStrLen(bestPatternString);
            /* only save 'y', 'M', 'd' charactor */
            for (; i < len; i++) {
                if (bestPatternString[i] == 'y' && ymd[0] == 0) {
                    region_format[j++] = bestPatternString[i];
                    ymd[0] = 1;
                } else if (bestPatternString[i] == 'M' && ymd[1] == 0) {
                    region_format[j++] = bestPatternString[i];
                    ymd[1] = 1;
                } else if (bestPatternString[i] == 'd' && ymd[2] == 0) {
                    region_format[j++] = bestPatternString[i];
                    ymd[2] = 1;
                }
            }

            region_format[3] = '\0';

            /* default is "Mdy" */
            int date_format_vconf_value = DATA_FORMAT_DEFAULT;
            for (i = 0; i < DATA_FORMAT_CATEGORY_NUM; i++) {
                if (!safeStrCmp(region_format, date_format_str[i]))
                    date_format_vconf_value = i;
            }

            /* if region_format is null, should be set as default */
            if (isEmptyStr(region_format) || isSpaceStr(region_format))
                date_format_vconf_value = 1;

            SETTING_TRACE("bestPatternString : %s, format: %s, index: %d", bestPatternString, region_format, date_format_vconf_value);

            setting_set_int_slp_key(INT_SLP_SETTING_DATE_FORMAT, date_format_vconf_value, &err);
            retv_if(err == SETTING_RETURN_FAIL, SETTING_RETURN_FAIL);

            G_FREE(ad->prev_locale);
            ad->prev_locale = (char *)g_strdup(region);
#endif
        }

        public static string GetDisplayLanguageName()
        {
            string title = "N/A";


            bool lang_automatic = Vconf.GetBool("db/setting/lang_automatic");
            if (lang_automatic)
            {
                return Resources.IDS_ST_BODY_ANSWERINGMODE_AUTOMATIC;
            }

            string locale = Vconf.GetString("db/menu_widget/language");
            String[] qStrings = locale.Split('.');
            locale = qStrings[0];

            foreach (DisplayLanguageInfo item in LanguageList)
            {
                if (locale == item.GetLocale())
                    return item.GetName();
            }

            return title;
        }
    }
}

