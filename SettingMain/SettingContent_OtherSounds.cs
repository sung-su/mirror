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
    class SettingContent_OtherSounds : SettingContent_Base
    {
        public SettingContent_OtherSounds()
            : base()
        {
            mTitle = Resources.IDS_ST_MBODY_OTHER_SOUNDS;
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


            // setting_vconf_get_bool(VCONFKEY_SETAPPL_TOUCH_SOUNDS_BOOL);
            string keyTouchSound = "db/setting/sound/touch_sounds";
            bool bTouchSound = Vconf.GetBool(keyTouchSound);
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_TOUCH_SOUND, Resources.IDS_ST_BODY_PLAY_SOUNDS_WHEN_LOCKING_AND_UNLOCKING_SCREEN, false, true);
            if (item != null)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = bTouchSound;


                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetBool(keyTouchSound, true);
                        Tizen.Log.Debug("NUI", "Touch Sound is ON!\n");
                    }
                    else
                    {
                        Vconf.SetBool(keyTouchSound, false);
                        Tizen.Log.Debug("NUI", "Touch Sound is OFF!\n");
                    }
                };

                content.Add(item);
            }

            // setting_vconf_get_bool(VCONFKEY_SETAPPL_BUTTON_SOUNDS_BOOL);
            string keyKeyboardSound = "db/setting/sound/button_sounds";
            bool bKeyboardSound = Vconf.GetBool(keyKeyboardSound);
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_KEYBOARD_SOUND, Resources.IDS_ST_BODY_SOUND_FEEDBACK_FOR_SYSTEM_KEYBOARD, false, true);
            if (item != null)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = bKeyboardSound;


                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetBool(keyKeyboardSound, true);
                        Tizen.Log.Debug("NUI", "Keyboard Sound is ON!\n");
                    }
                    else
                    {
                        Vconf.SetBool(keyKeyboardSound, false);
                        Tizen.Log.Debug("NUI", "Keyboard Sound is OFF!\n");
                    }
                };

                content.Add(item);
            }


            return content;
        }

    }
}
