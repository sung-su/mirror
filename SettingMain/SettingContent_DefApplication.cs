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
    class SettingContent_DefApplication : SettingContent_Base
    {

        private DefaultLinearItem mFontsizeItem;
        private DefaultLinearItem mFonttypeItem;




        public SettingContent_DefApplication()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_DEFAULT_APPLICATIONS_ABB;

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


            //VCONFKEY_SETAPPL_SELECTED_PACKAGE_NAME
            string appID = Vconf.GetString("db/setting/menuscreen/package_name");
            ApplicationInfo appinfo = new ApplicationInfo(appID);

            content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_LAUNCH_BY_DEFAULT));
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_HOME, appinfo.Label);
            mFontsizeItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestAppLaunch("org.tizen.setting-homescreen");
                };
                content.Add(item);
            }


            content.Add(SettingItemCreator.CreateItemStatic(""));
            content.Add(SettingItemCreator.CreateItemStatic(""));

            content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_CLEAR_DEFAULTS));
            item = SettingItemCreator.CreateItemWithCheck("", Resources.IDS_ST_BODY_THERE_ARE_NO_APPS_SET_AS_DEFAULTS);
            mFonttypeItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {

                };
                content.Add(item);
            }

            return content;
        }
    }
}
