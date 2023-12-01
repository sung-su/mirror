using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingView
{
    public class CustomPage : Page
    {
        public TextLabel Title { get; set; }

        public ScrollableBase Content { get; set;  }

        public CustomPage()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.MatchParent;
            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
            };
            ThemeChangeSensitive = true;
            CornerRadius = (SettingViewBorder.WindowCornerRadius - SettingViewBorder.WindowPadding).SpToPx();
        }
    }
}
