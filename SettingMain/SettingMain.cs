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
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

namespace SettingMain
{
    public class Program : NUIWidgetApplication
    {

        public static readonly string ItemContentNameIcon = "ItemIcon";
        public static readonly string ItemContentNameTitle = "ItemTitle";
        public static readonly string ItemContentNameDescription = "ItemDescription";

        public Program(Dictionary<System.Type, string> widgetSet) : base(widgetSet)
        {

        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        static void Main(string[] args)
        {
            Dictionary<System.Type, string> widgetSet = new Dictionary<Type, string>();
            
            widgetSet.Add(typeof(SettingContent_Sound), "sound@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_Soundmode), "soundmode@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_NotificationSound), "notificationsound@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_OtherSounds), "othersounds@org.tizen.cssettings");
            
            widgetSet.Add(typeof(SettingContent_Display), "display@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_Font), "font@org.tizen.cssettings");
                    widgetSet.Add(typeof(SettingContent_Fontsize), "fontsize@org.tizen.cssettings");
                    widgetSet.Add(typeof(SettingContent_Fonttype), "fonttype@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_ScreenTimeout), "timeout@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_Theme), "theme@org.tizen.cssettings");
            
            widgetSet.Add(typeof(SettingContent_Applications), "apps@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_DefApplication), "defapplication@org.tizen.cssettings");

            widgetSet.Add(typeof(SettingContent_Storage), "storage@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_DefaultStorage), "defaultstorage@org.tizen.cssettings");
            
            widgetSet.Add(typeof(SettingContent_LanguageInput), "languageinput@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_DisplayLanguage), "displaylanguage@org.tizen.cssettings");
            
            widgetSet.Add(typeof(SettingContent_DateTime), "datetime@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_SetDate), "setdate@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_SetTime), "settime@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_SetTimezone), "settimezone@org.tizen.cssettings");
            
            widgetSet.Add(typeof(SettingContent_AboutDevice), "aboutdevice@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_LegalInfo), "legalinfo@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_RenameDevice), "renamedevice@org.tizen.cssettings");
                widgetSet.Add(typeof(SettingContent_DeviceStatus), "devicestatus@org.tizen.cssettings");


            var app = new Program(widgetSet);
            app.Run(args);
        }
    }
}
