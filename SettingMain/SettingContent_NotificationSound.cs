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

    

    class SettingContent_NotificationSound : SettingContent_Base
    {

        static public string GetFileName(string value)
        {
            String[] folders = value.Split('/');
            int foldercount = folders.Length;
            if (foldercount > 0) return folders[foldercount - 1];
            return value;
        }


        private ArrayList SoundList = null;



        private void MakeSoundList() 
        {
            SoundList = new ArrayList();


            string sharedData = "/opt/usr/data";
            string path = sharedData + "/settings/Alerts";

            Tizen.Log.Debug("NUI", String.Format("sound path : {0}", path));

            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(path);

            int i = 0;

            FileInfo[] wavFiles = d.GetFiles("*.wav");
            foreach (FileInfo file in wavFiles)
            {
                Tizen.Log.Debug("NUI", String.Format("[{0}] {1}", i, file.Name));
                SoundList.Add(path +"/"+ file.Name);
                i++;
            }

            FileInfo[] mp3Files = d.GetFiles("*.mp3");
            foreach (FileInfo file in mp3Files)
            {
                Tizen.Log.Debug("NUI", String.Format("[{0}] {1}", i, file.Name));
                SoundList.Add(path + "/" + file.Name);
                i++;
            }

            
        }


        private string[] PickerItems;
        public SettingContent_NotificationSound()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            // Init data list
            MakeSoundList();
            // Make menu list
            PickerItems = new string[SoundList.Count];
            for (int i = 0; i < SoundList.Count; i++)
            {
                string path = SoundList[i] as string;
                PickerItems[i] = GetFileName(path);
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
            picker.CurrentValue = GetNotificationSoundIndex();
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

                SetNotificationSoundIndex(picker.CurrentValue);

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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_NOTIFICATION));
            content.Add(picker);
            content.Add(button);

            return content;
        }


        public int GetNotificationSoundIndex()
        {
            string sound = GetNotificationSound();


            for(int i = 0; i < SoundList.Count; i++) {
                if (sound.Equals(SoundList[i])) {
                    return i;
                }

            }

            return -1;
        }

        private void SetNotificationSoundIndex(int index)
        {
            string path = SoundList[index] as string;
            SetNotificationSound(path);
        }


        private static void SetNotificationSound(string notificationsound)
        {
            Vconf.SetString("db/setting/sound/noti/msg_ringtone_path", notificationsound);
        }

        public static string GetNotificationSound()
        {
            return Vconf.GetString("db/setting/sound/noti/msg_ringtone_path");
        }
        public static string GetNotificationSoundName()
        {
            string path = GetNotificationSound();
            String[] folders = path.Split('/');
            int foldercount = folders.Length;
            if (foldercount > 0) return folders[foldercount - 1];
            return path;
        }
    }
}

