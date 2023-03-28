using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingCore.Views;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu.Sound
{
    public class SoundotherGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_MBODY_OTHER_SOUNDS;

        private const string keyTouchSound = "db/setting/sound/touch_sounds";
        private const string keyKeyboardSound = "db/setting/sound/button_sounds";

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            Tizen.Vconf.TryGetBool(keyTouchSound, out bool bTouchSound);
            Tizen.Vconf.TryGetBool(keyKeyboardSound, out bool bKeyboardSound);

            var item = new SwitchListItem(Resources.IDS_ST_MBODY_TOUCH_SOUND, Resources.IDS_ST_BODY_PLAY_SOUNDS_WHEN_LOCKING_AND_UNLOCKING_SCREEN, bTouchSound);
            item.Switch.SelectedChanged += (o, e) =>
            {
                Tizen.Vconf.SetBool(keyTouchSound, e.IsSelected);
                Logger.Debug($"Touch Sound enabled: {e.IsSelected}");
            };
            content.Add(item);

            item = new SwitchListItem(Resources.IDS_ST_MBODY_KEYBOARD_SOUND, Resources.IDS_ST_BODY_SOUND_FEEDBACK_FOR_SYSTEM_KEYBOARD, bKeyboardSound);
            item.Switch.SelectedChanged += (o, e) =>
            {
                Tizen.Vconf.SetBool(keyKeyboardSound, e.IsSelected);
                Logger.Debug($"Keyboard Sound enabled: {e.IsSelected}");
            };
            content.Add(item);

            return content;
        }
    }
}
