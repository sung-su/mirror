using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

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

        public SettingContent_DisplayLanguage()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE;
        }

        protected override View CreateContent(Window window)
        {
            // Content of the page which scrolls items vertically.
            var content = new ScrollableBase()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            DefaultLinearItem item = null;

            foreach (var lang in LanguageList) 
            {
                item = CreateItemWithCheck(lang.GetName());
                content.Add(item);

            }

            return content;
        }

    }
}
