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
using System.IO;
using System.Collections;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using System.Collections.ObjectModel;
using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{

    

    class SettingContent_Fonttype : SettingContent_Base
    {

        static public string GetFileName(string value)
        {
            String[] folders = value.Split('/');
            int foldercount = folders.Length;
            if (foldercount > 0) 
                return folders[foldercount - 1];
            return value;
        }


        private static ArrayList FonttypeList = null;



        private static void MakeFonttypeList() 
        {
            FonttypeList = new ArrayList();

            FonttypeList.Add(SystemSettings.FontType);
            Tizen.Log.Debug("NUI", "SystemSettings.DefaultFontType : "+ SystemSettings.DefaultFontType);
            Tizen.Log.Debug("NUI", "SystemSettings.FontType : " + SystemSettings.FontType);

        }


        private string[] PickerItems;
        public SettingContent_Fonttype()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            // Init data list
            MakeFonttypeList();

            // Make menu list
            PickerItems = new string[FonttypeList.Count];
            for (int i = 0; i < FonttypeList.Count; i++)
            {
                string type = FonttypeList[i] as string;
                PickerItems[i] = GetFileName(type);
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
            picker.CurrentValue = GetFonttypeIndex();
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

                SetFonttypeIndex(picker.CurrentValue);

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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_FONT_TYPE));
            content.Add(picker);
            content.Add(button);

            return content;
        }


        public static int GetFonttypeIndex()
        {
            string fonttype = GetFonttype();


            for(int i = 0; i < FonttypeList.Count; i++) {
                if (fonttype.Equals(FonttypeList[i])) {
                    return i;
                }

            }

            return -1;
        }

        private static void SetFonttypeIndex(int index)
        {
            string type = FonttypeList[index] as string;
            SetFonttype(type);
        }


        private static void SetFonttype(string fonttype)
        {
            try
            {
                if (fonttype.Equals("Default")) {
                    fonttype = SystemSettings.DefaultFontType;
                }
                SystemSettings.FontType = fonttype;
            }
            catch (Exception e)
            {
                Tizen.Log.Debug("NUI", string.Format("error :({0}) {1} ", e.GetType().ToString(), e.Message));
            }

        }

        public static string GetFonttype()
        {
            return SystemSettings.FontType;
        }
        public static string GetFonttypeName()
        {
            return GetFonttype();
        }
    }
}

