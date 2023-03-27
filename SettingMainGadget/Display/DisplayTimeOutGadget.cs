using SettingAppTextResopurces.TextResources;
using SettingMainGadget.Display;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Display
{
    public class DisplayTimeOutGadget : SettingCore.MenuGadget
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
                    HorizontalAlignment = HorizontalAlignment.Begin,
                    VerticalAlignment = VerticalAlignment.Top,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            var timeoutList = DisplayTimeOutManager.TimeoutList.Select(x => x.GetName()).ToList();

            for (int i = 0; i < timeoutList.Count; i++)
            {
                RadioButton radioButton = new RadioButton()
                {
                    Text = timeoutList[i],
                    ThemeChangeSensitive = true,
                    IsSelected = i.Equals(DisplayTimeOutManager.GetScreenTimeoutIndex()),
                    Margin = new Extents(24, 0, 0, 0).SpToPx(),
                };

                radioButtonGroup.Add(radioButton);
                content.Add(radioButton);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                DisplayTimeOutManager.SetScreenTimeout(radioButtonGroup.SelectedIndex);
            };

            return content;
        }
    }
}
