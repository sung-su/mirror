using SettingCore.TextResources;
using SettingCore.Views;
using SettingMainGadget.Sound;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Sound
{
    public class SoundmodeGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_HEADER_SOUND_MODE;

        private SettingMainGadget.Sound.Soundmode[] sounds = new[] {
            Soundmode.SOUND_MODE_SOUND,
            Soundmode.SOUND_MODE_VIBRATE,
            Soundmode.SOUND_MODE_MUTE,
        };

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

            for (int i = 0; i < sounds.Length; i++)
            {
                RadioButtonListItem item = new RadioButtonListItem(SoundmodeManager.GetSoundmodeName(sounds[i]));
                item.RadioButton.IsSelected = i.Equals(sounds.ToList().IndexOf(SoundmodeManager.GetSoundmode()));

                radioButtonGroup.Add(item.RadioButton);
                content.Add(item);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                var current = sounds[radioButtonGroup.SelectedIndex];
                SoundmodeManager.SetSoundmode(current);
            };

            return content;
        }
    }
}
