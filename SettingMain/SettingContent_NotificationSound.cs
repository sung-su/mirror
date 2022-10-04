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


        private static ArrayList SoundList = null;



        private static void MakeSoundList() 
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
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                // Size = new Size(100, 200),
            };

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(PickerItems);
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = PickerItems.Length - 1;
            picker.CurrentValue = GetNotificationSoundIndex();
            Tizen.Log.Debug("NUI", "DisplayedValues : " + picker.DisplayedValues);

            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {

                Tizen.Log.Debug("NUI", String.Format("current : {0}", PickerItems[picker.CurrentValue]));

                SetNotificationSoundIndex(picker.CurrentValue);

                // Update Widget Content by sending message to pop the fourth page.
                Bundle nextBundle2 = new Bundle();
                nextBundle2.AddItem("WIDGET_ACTION", "POP");
                String encodedBundle2 = nextBundle2.Encode();
                SetContentInfo(encodedBundle2);
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


        public static int GetNotificationSoundIndex()
        {
            string sound = GetNotificationSound();


            for(int i = 0; i < SoundList.Count; i++) {
                if (sound.Equals(SoundList[i])) {
                    return i;
                }

            }

            return -1;
        }

        private static void SetNotificationSoundIndex(int index)
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

