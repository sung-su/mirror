using SettingMainGadget.TextResources;
using SettingCore;
using SettingMainGadget.Sound;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using SettingCore.Views;

namespace Setting.Menu.Sound
{
    public class SoundnotificationGadget : MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NOTIFICATIONS));

        private readonly string soundpath = Directory.GetParent(Tizen.System.SystemSettings.EmailAlertRingtone).FullName;

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new ScrollableBase
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

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            var soundList = CreateSoundList();
            int selectedIndex = GetNotificationSoundIndex(soundList);


            for (int i = 0; i < soundList.Count; i++)
            {
                string displayName = SoundNotificationManager.SettingMediaBasename(this, soundList[i].ToString());
                var item = new RadioButtonListItem(displayName)
                {
                    RadioButton =
                        {
                            IsSelected = i == selectedIndex
                        }
                };

                radioButtonGroup.Add(item.RadioButton);
                content.Add(item);
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

        private static int GetNotificationSoundIndex(IList<string> soundlist)
        {
            string sound = SoundNotificationManager.GetNotificationSound();
            return soundlist.IndexOf(sound);
        }
    }
}
