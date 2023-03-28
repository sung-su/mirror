/*
 *  Copyright (c) 2022 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingCore.TextResources;

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

            content.Add(SettingItemCreator.CreateItemStatic(""));
            content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_KEYBOARD));

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_KEYBOARD);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("inputmethod@org.tizen.cssetting-inputmethod");
                };
                content.Add(item);
            }

            content.Add(SettingItemCreator.CreateItemStatic(""));
            content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_INPUT_ASSISTANCE));

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_AUTOFILL_SERVICE);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("autofill@org.tizen.cssetting-autofill");
                };
                content.Add(item);
            }

            content.Add(SettingItemCreator.CreateItemStatic(""));
            content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_SPEECH));
            
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
