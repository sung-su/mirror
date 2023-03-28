using SettingCore.TextResources;
using SettingCore;
using SettingMainGadget.Sound;
using System.Collections.Generic;
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

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Begin,
                    VerticalAlignment = VerticalAlignment.Top,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            var soundList = CreateSoundList();

            for (int i = 0; i < soundList.Count; i++)
            {
                RadioButton radioButton = new RadioButton()
                {
                    ThemeChangeSensitive = true,
                    Text = SoundNotificationManager.SettingMediaBasename(soundList[i].ToString()),
                    IsSelected = i.Equals(GetNotificationSoundIndex(soundList)),
                    Margin = new Extents(24, 0, 0, 0).SpToPx(),
                };

                radioButtonGroup.Add(radioButton);
                content.Add(radioButton);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                SoundNotificationManager.SetNotificationSound(soundList[radioButtonGroup.SelectedIndex]);
            };

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
