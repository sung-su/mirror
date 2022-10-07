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
    class SettingContent_LanguageInput : SettingContent_Base
    {
        public SettingContent_LanguageInput()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT;
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

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE, SettingContent_DisplayLanguage.GetDisplayLanguageName());
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("displaylanguage@org.tizen.cssettings");
                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_KEYBOARD);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("inputmethod@org.tizen.cssetting-inputmethod");
                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_AUTOFILL_SERVICE);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("autofill@org.tizen.cssetting-autofill");
                };
                content.Add(item);
            }



            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_BODY_VOICE_CONTROL_ABB2);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("voicecontrol@org.tizen.cssetting-voicecontrol");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_HEADER_TEXT_TO_SPEECH_HTTS);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("tts@org.tizen.cssetting-tts");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_HEADER_SPEECH_TO_TEXT_HSTT);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("stt@org.tizen.cssetting-stt");
                };
                content.Add(item);
            }


            return content;
        }

        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);

            Tizen.System.SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Tizen.System.SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;

            base.OnTerminate(contentInfo, type);
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, Tizen.System.LocaleLanguageChangedEventArgs e)
        {
            if (mPage != null)
            {
                mPage.AppBar.Title = Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT;
                mPage.Content = CreateContent(mWindow);
            }
        }
    }
}
