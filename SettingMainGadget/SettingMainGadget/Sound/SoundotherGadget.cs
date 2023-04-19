using SettingMainGadget.TextResources;
using SettingCore;
using SettingCore.Views;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Sound
{
    public class SoundotherGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_OTHER_SOUNDS));

        private const string keyTouchSound = "db/setting/sound/touch_sounds";
        private const string keyKeyboardSound = "db/setting/sound/button_sounds";

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

            Tizen.Vconf.TryGetBool(keyTouchSound, out bool bTouchSound);
            Tizen.Vconf.TryGetBool(keyKeyboardSound, out bool bKeyboardSound);

            var item = new SwitchListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_TOUCH_SOUND)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_PLAY_SOUNDS_WHEN_LOCKING_AND_UNLOCKING_SCREEN)), bTouchSound);
            item.Switch.SelectedChanged += (o, e) =>
            {
                Tizen.Vconf.SetBool(keyTouchSound, e.IsSelected);
                Logger.Debug($"Touch Sound enabled: {e.IsSelected}");
            };
            content.Add(item);

            item = new SwitchListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_KEYBOARD_SOUND)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SOUND_FEEDBACK_FOR_SYSTEM_KEYBOARD)), bKeyboardSound);
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
