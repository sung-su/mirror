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
using System.Collections.ObjectModel;
using Tizen.System;

using SettingCore.TextResources;

namespace SettingMain
{

    

    class SettingContent_Soundmode_Temp : SettingContent_Base
    {
        public enum EnumSoundmode
        {
            SOUND_MODE_SOUND,
            SOUND_MODE_VIBRATE,
            SOUND_MODE_MUTE
        };

        public class SoundmodeInfo
        {
            private readonly string Name = null;
            private readonly EnumSoundmode Value;


            public SoundmodeInfo(string name, EnumSoundmode value)
            {
                Name = name;
                Value = value;
            }


            public string GetName()
            {
                return Name;
            }

            public EnumSoundmode GetValue()
            {
                return Value;
            }
        };


        private static readonly SoundmodeInfo[] SoundmodeList =
        {
            new SoundmodeInfo(SoundmodeToString(EnumSoundmode.SOUND_MODE_SOUND), EnumSoundmode.SOUND_MODE_SOUND),
            new SoundmodeInfo(SoundmodeToString(EnumSoundmode.SOUND_MODE_MUTE), EnumSoundmode.SOUND_MODE_MUTE),
        };



        private string[] PickerItems;
        public SettingContent_Soundmode_Temp()
            : base()
        {


            mTitle = Resources.IDS_ST_HEADER_SOUND_MODE;
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

            int indexCurrent = GetSoundmodeIndex();


            item = SettingItemCreator.CreateItemWithCheck(SoundmodeList[0].GetName());
            if (item != null)
            {
                if (indexCurrent == 0)
                {
                    item.Label.TextColor = Color.DarkRed;
                }
                item.Clicked += (o, e) =>
                {
                    SetSoundmodeIndex(0);

                    RequestWidgetPop();
                };

                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithCheck(SoundmodeList[1].GetName());
            if (item != null)
            {
                if (indexCurrent == 1)
                {
                    item.Label.TextColor = Color.DarkRed;
                }
                item.Clicked += (o, e) =>
                {
                    SetSoundmodeIndex(1);

                    RequestWidgetPop();
                };

                content.Add(item);
            }

            return content;
        }


        public static int GetSoundmodeIndex()
        {
            EnumSoundmode mode = GetSoundmode();

            if (mode == EnumSoundmode.SOUND_MODE_SOUND) 
                return 0;
            return 1;
        }

        private static void SetSoundmodeIndex(int index)
        {
            SetSoundmode(SoundmodeList[index].GetValue());
        }


        private static void SetSoundmode(EnumSoundmode soundmode)
        {
            bool have_sound = false, have_vibrations = false;

            switch (soundmode)
            {
                case EnumSoundmode.SOUND_MODE_SOUND:
                    have_sound = true;
                    have_vibrations = false;
                    break;
                case EnumSoundmode.SOUND_MODE_VIBRATE:
                    have_sound = false;
                    have_vibrations = true;
                    break;
                case EnumSoundmode.SOUND_MODE_MUTE:
                    have_sound = false;
                    have_vibrations = false;
                    break;
            };

            Vconf.SetBool("db/setting/sound/sound_on", have_sound);
            Vconf.SetBool("db/setting/sound/vibration_on", have_vibrations);
        }

        public static EnumSoundmode GetSoundmode()
        {
            bool have_sound = Vconf.GetBool("db/setting/sound/sound_on");
                
                
            bool have_vibrations = Vconf.GetBool("db/setting/sound/vibration_on");

            if (have_sound)
                return EnumSoundmode.SOUND_MODE_SOUND;
            else if (have_vibrations)
                return EnumSoundmode.SOUND_MODE_VIBRATE;
            
            return EnumSoundmode.SOUND_MODE_MUTE;
        }
        public static string GetSoundmodeName()
        {
            return SoundmodeToString(GetSoundmode());

        }


        public static string SoundmodeToString(EnumSoundmode mode)
        {
            return mode switch
            {
                EnumSoundmode.SOUND_MODE_SOUND => Resources.IDS_ST_HEADER_SOUND,
                EnumSoundmode.SOUND_MODE_VIBRATE => Resources.IDS_ST_HEADER_VIBRATE,
                EnumSoundmode.SOUND_MODE_MUTE => Resources.IDS_ST_HEADER_MUTE,
                _ => null,
            };
        }

    }
}

