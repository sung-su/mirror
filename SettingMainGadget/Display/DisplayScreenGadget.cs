using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMainGadget.Display;
using System.Collections.ObjectModel;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Display
{
    public class DisplayscreenGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2;

        protected override View OnCreate()
        {
            base.OnCreate();

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

            var picker = new Picker()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
            };

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(DisplayscreenManager.TimeoutList.Select(x => x.GetName()).ToList());
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = DisplayscreenManager.TimeoutList.Count - 1;
            picker.CurrentValue = DisplayscreenManager.GetScreenTimeoutIndex();

            Logger.Debug($"ScreenTimeOut CurrentValue: {picker.CurrentValue}");

            var button = new Button()
            {
                Text = Resources.IDS_ST_BUTTON_OK
            };

            button.Clicked += (bo, be) =>
            {
                DisplayscreenManager.SetScreenTimeout(picker.CurrentValue);

                NavigateBack();
            };

            content.Add(picker);
            content.Add(button);

            return content;
        }
    }
}
