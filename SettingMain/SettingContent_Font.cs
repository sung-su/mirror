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

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_Font : SettingContent_Base
    {

        private DefaultLinearItem mFontsizeItem;
        private DefaultLinearItem mFonttypeItem;
        public SettingContent_Font()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_FONT;

            mFontsizeItem = null;
            mFonttypeItem = null;
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

            //SystemSettingsFontSize fontSize = SystemSettings.FontSize;
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_FONT_SIZE, SettingContent_Fontsize.GetFontsizeName());
            mFontsizeItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("fontsize@org.tizen.cssettings");
                };
                content.Add(item);
            }

            //string fontType = SystemSettings.FontType;
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_FONT_TYPE, SettingContent_Fonttype.GetFonttypeName());
            mFonttypeItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("fonttype@org.tizen.cssettings");
                };
                content.Add(item);
            }

            return content;
        }

        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);

            Tizen.System.SystemSettings.FontSizeChanged += SystemSettings_FontSizeChanged;
            Tizen.System.SystemSettings.FontTypeChanged += SystemSettings_FontTypeChanged;
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Tizen.System.SystemSettings.FontSizeChanged -= SystemSettings_FontSizeChanged;
            Tizen.System.SystemSettings.FontTypeChanged -= SystemSettings_FontTypeChanged;

            base.OnTerminate(contentInfo, type);
        }

        private void SystemSettings_FontSizeChanged(object sender, FontSizeChangedEventArgs e)
        {
            mFontsizeItem.SubText = SettingContent_Fontsize.GetFontsizeName();
        }

        private void SystemSettings_FontTypeChanged(object sender, FontTypeChangedEventArgs e)
        {
            mFonttypeItem.SubText = SettingContent_Fonttype.GetFonttypeName();
        }


    }
}
