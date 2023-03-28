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
    public class ScreenTimeoutInfo
    {
        private readonly string Name = null;
        private readonly int Value;


        public ScreenTimeoutInfo(string name, int value)
        {
            Name = name;
            Value = value;
        }


        public string GetName()
        {
            return Name;
        }

        public int GetValue()
        {
            return Value;
        }
    };


    class SettingContent_ScreenTimeout : SettingContent_Base
    {

        private static readonly ScreenTimeoutInfo[] TimeoutList =
        {
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_ALWAYS_ON, 0),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_15SEC, 15),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_30SEC, 30),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_1_MINUTE, 60),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_2_MINUTES, 120),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_5_MINUTES, 300),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_10_MINUTES, 600)
        };



        private string[] PickerItems;
        public SettingContent_ScreenTimeout()
            : base()
        {


            mTitle = Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2;

            PickerItems = new string[TimeoutList.Length];
            for (int i = 0; i < TimeoutList.Length; i++)
            {
                PickerItems[i] = TimeoutList[i].GetName();
            }
        }

        protected override View CreateContent(Window window)
        {
            var picker = new Picker()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(PickerItems);
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = PickerItems.Length - 1;
            picker.CurrentValue = GetScreenTimeoutIndex();
            Tizen.Log.Debug("NUI", "CurrentValue : " + picker.CurrentValue.ToString());

            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {

                Tizen.Log.Debug("NUI", String.Format("current : {0}", picker.CurrentValue));

                SetScreenTimeout(picker.CurrentValue);

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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2));
            content.Add(picker);
            content.Add(button);

            return content;
        }

        void SetScreenTimeout(int index)
        {
            SystemSettings.ScreenBacklightTime = TimeoutList[index].GetValue();
        }

        public static int GetScreenTimeoutIndex()
        {
            //int value = SystemSettings.ScreenBacklightTime;
            int value = Vconf.GetInt("db/setting/lcd_backlight_normal");

            Tizen.Log.Debug("NUI", "ScreenTimeout : " + value.ToString());

            int index;
            if (value < 15)
            {
                index = 0;
            }
            else if (value >= 15 && value < 30)
            {
                index = 1;
            }
            else if (value >= 30 && value < 60)
            {
                index = 2;
            }
            else if (value >= 60 && value < 120)
            {
                index = 3;
            }
            else if (value >= 120 && value < 300)
            {
                index = 4;
            }
            else if (value >= 300 && value < 600)
            {
                index = 5;
            }
            else
            {
                index = 6;
            }

            return index;
        }
        public static string GetScreenTimeoutName()
        {
            return TimeoutList[GetScreenTimeoutIndex()].GetName();

        }
    }
}

