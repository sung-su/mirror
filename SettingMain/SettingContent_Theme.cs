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
    public class ThemeInfo
    {
        private string Name;
        private string Id;


        public ThemeInfo(string name, string id)
        {
            Name = name;
            Id = id;
        }


        public string GetName()
        {
            return Name;
        }

        public string GetId()
        {
            return Id;
        }
    };


    class SettingContent_Theme : SettingContent_Base
    {

        private static readonly ThemeInfo[] ThemeList =
        {
            new ThemeInfo("Light theme", "org.tizen.default-light-theme"),
            new ThemeInfo("Dark theme", "org.tizen.default-dark-theme"),
        };

        private string[] PickerItems;
        public SettingContent_Theme()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            PickerItems = new string[ThemeList.Length];
            for (int i = 0; i < ThemeList.Length; i++)
            {
                PickerItems[i] = ThemeList[i].GetName();
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
            int index = GetThemeIndex();
            picker.CurrentValue = (index<0)? 0 : index;
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

                SetThemeIndex(picker.CurrentValue);

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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_THEME));
            content.Add(picker);
            content.Add(button);

            return content;
        }

        void SetThemeIndex(int index)
        {
            if(index>=0 && index< ThemeList.Length)
                SetTheme(ThemeList[index].GetId());
        }
        void SetTheme(string Id)
        {
            ThemeManager.ApplyPlatformTheme(Id);
        }

        public static int GetThemeIndex()
        {
            string curId = GetTheme();
            if (string.IsNullOrEmpty(curId))
            {
                Tizen.Log.Debug("NUI", "Theme : Not Available");
                return -1;
            }

            Tizen.Log.Debug("NUI", "Theme : " + curId);

            int i = 0;
            foreach (var theme in ThemeList)
            {
                if (curId.Equals(theme.GetId()))
                    return i;
                i++;
            }

            return -1;
        }
        public static string GetThemeName()
        {
            int index = GetThemeIndex();
            if(index >= 0)
                return ThemeList[index].GetName();
            
            return Resources.IDS_ST_HEADER_UNAVAILABLE;

        }

        public static string GetTheme()
        {
            return ThemeManager.PlatformThemeId;
        }
    }
}