using SettingMainGadget.TextResources;
using SettingCore.Views;
using SettingMainGadget.Display;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Display
{
    public class DisplayThemeGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_THEME));

        protected override View OnCreate()
        {
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

            var themeList = DisplayThemeManager.ThemeList.Select(a => a.GetName(this)).ToList();

            for (int i = 0; i < themeList.Count; i++)
            {
                RadioButtonListItem item = new RadioButtonListItem(themeList[i]);
                item.RadioButton.IsSelected = i.Equals(DisplayThemeManager.GetThemeIndex());

                radioButtonGroup.Add(item.RadioButton);
                content.Add(item);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                DisplayThemeManager.SetTheme(DisplayThemeManager.ThemeList[radioButtonGroup.SelectedIndex].GetId());
            };

            return content;
        }
    }
}
