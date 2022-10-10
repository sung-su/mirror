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
    class SettingContent_Storage : SettingContent_Base
    {
        public SettingContent_Storage()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_SOUND;
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

            DefaultLinearItem item;

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_DEVICE_STORAGE);
            content.Add(item);

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_USED, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_TOTAL_SPACE, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_SM_BODY_FREE_M_MEMORY_ABB, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            return content;
        }

    }
}
