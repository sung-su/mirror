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

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{

    

    class SettingContent_Fontsize : SettingContent_Base
    {

#if false
        public enum SystemSettingsFontSize
        {
            Small = 0,
            Normal = 1,
            Large = 2,
            Huge = 3,
            Giant = 4
        }
#endif


        public class FontsizeInfo
        {
            private readonly SystemSettingsFontSize Value;


            public FontsizeInfo(SystemSettingsFontSize value)
            {
                Value = value;
            }


            public string GetName()
            {
                return Value.ToString();
            }

            public SystemSettingsFontSize GetValue()
            {
                return Value;
            }
        };


        private static readonly FontsizeInfo[] FontsizeList =
        {
            new FontsizeInfo(SystemSettingsFontSize.Small),
            new FontsizeInfo(SystemSettingsFontSize.Normal),
            new FontsizeInfo(SystemSettingsFontSize.Large),
            new FontsizeInfo(SystemSettingsFontSize.Huge),
            new FontsizeInfo(SystemSettingsFontSize.Giant)
        };



        private string[] PickerItems;
        public SettingContent_Fontsize()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            PickerItems = new string[FontsizeList.Length];
            for (int i = 0; i < FontsizeList.Length; i++)
            {
                PickerItems[i] = FontsizeList[i].GetName();
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
            picker.CurrentValue = GetFontsizeIndex();
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

                SetFontsizeIndex(picker.CurrentValue);

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
            content.Add(new TextLabel(Resources.IDS_ST_MBODY_FONT_SIZE));
            content.Add(picker);
            content.Add(button);

            return content;
        }


        public static int GetFontsizeIndex()
        {
            SystemSettingsFontSize cursize = GetFontsize();

            int i = 0;
            foreach (FontsizeInfo sizeinfo in FontsizeList) {
                if (sizeinfo.GetValue() == cursize)
                    return i;
                i++;
            }

            return -1;
        }

        private static void SetFontsizeIndex(int index)
        {
            SetFontsize(FontsizeList[index].GetValue());
        }


        private static void SetFontsize(SystemSettingsFontSize fontsize)
        {
            SystemSettings.FontSize = fontsize;
        }

        public static SystemSettingsFontSize GetFontsize()
        {
            return SystemSettings.FontSize;
        }
        public static string GetFontsizeName()
        {
            return GetFontsize().ToString();
        }

    }
}

