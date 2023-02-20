using SettingAppTextResopurces.TextResources;
using SettingMainGadget.Sound;
using System.Collections.ObjectModel;
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

            var picker = new Picker()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent, // FIXME: required for Picker to draw correctly
            };

            picker.DisplayedValues = new ReadOnlyCollection<string>(sounds.Select(s => SoundmodeManager.GetSoundmodeName(s)).ToArray());
            picker.MinValue = 0;
            picker.MaxValue = sounds.Length - 1;
            picker.CurrentValue = sounds.ToList().IndexOf(SoundmodeManager.GetSoundmode());

            var button = new Button()
            {
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                var current = sounds[picker.CurrentValue];
                SoundmodeManager.SetSoundmode(current);

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
    }
}
