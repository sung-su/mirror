using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMainGadget.Sound;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Sound
{
    public class SoundnotificationGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_NOTIFICATIONS;

        private const string soundpath = "/opt/usr/data/settings/Alerts";
        private string[] pickerItems;

        protected override View OnCreate()
        {
            base.OnCreate();

            var picker = new Picker()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent, // FIXME: required for Picker to draw correctly
            };

            var soundList = CreateSoundList();

            pickerItems = new string[soundList.Count];
            for (int i = 0; i < soundList.Count; i++)
            {
                pickerItems[i] = SoundNotificationManager.SettingMediaBasename(soundList[i].ToString());
            }

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(pickerItems);
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = pickerItems.Length - 1;
            picker.CurrentValue = GetNotificationSoundIndex(soundList);

            var button = new Button()
            {
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                SoundNotificationManager.SetNotificationSound(soundList[picker.CurrentValue]);

                NavigateBack();
            };

            var content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            content.Add(picker);
            content.Add(button);

            return content;
        }

        private IList<string> CreateSoundList()
        {
            var soundList = new List<string>() { string.Empty }; // Silent

            Logger.Debug($"sound path : {soundpath}");

            DirectoryInfo directory = new DirectoryInfo(soundpath);
            var files = Enumerable.Concat(directory.GetFiles("*.wav"), directory.GetFiles("*.mp3"));

            foreach (var file in files)
            {
                Logger.Debug($"[{soundList.Count}] {file.Name}");
                soundList.Add(System.IO.Path.Combine(soundpath, file.Name));
            }

            return soundList;
        }

        private int GetNotificationSoundIndex(IList<string> soundlist)
        {
            string sound = SoundNotificationManager.GetNotificationSound();

            return soundlist.IndexOf(sound);
        }
    }
}
