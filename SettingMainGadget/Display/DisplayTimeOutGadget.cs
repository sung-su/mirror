using SettingCore.TextResources;
using SettingCore.Views;
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
                RadioButtonListItem item = new RadioButtonListItem(timeoutList[i]);
                item.RadioButton.IsSelected = i.Equals(DisplayTimeOutManager.GetScreenTimeoutIndex());

                radioButtonGroup.Add(item.RadioButton);
                content.Add(item);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                DisplayTimeOutManager.SetScreenTimeout(radioButtonGroup.SelectedIndex);
            };

            return content;
        }
    }
}
