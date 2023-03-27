using SettingAppTextResopurces.TextResources;
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

            for (int i = 0; i < sounds.Length; i++)
            {
                RadioButton radioButton = new RadioButton()
                {
                    ThemeChangeSensitive = true,
                    Text = SoundmodeManager.GetSoundmodeName(sounds[i]),
                    IsSelected = i.Equals(sounds.ToList().IndexOf(SoundmodeManager.GetSoundmode())),
                    Margin = new Extents(24, 0, 0, 0).SpToPx(),
                };

                radioButtonGroup.Add(radioButton);
                content.Add(radioButton);
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
